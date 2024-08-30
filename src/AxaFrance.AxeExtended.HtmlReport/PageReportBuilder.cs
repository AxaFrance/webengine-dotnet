using Deque.AxeCore.Commons;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("AxaFrance.AxeExtended.Selenium")]
namespace AxaFrance.AxeExtended.HtmlReport
{
    /// <summary>
    /// Classes for generating HTML report from given Axe-Core results
    /// </summary>
    public class PageReportBuilder
    {
        /// <summary>
        /// Options for the page report.
        /// </summary>
        public PageReportOptions Options { get; set; }


        /// <summary>
        /// Provide a custom axe configure (https://github.com/dequelabs/axe-core/blob/master/doc/API.md#api-name-axeconfigure).
        /// If provided, the configuration will be used. If not provided, the default axe configuration will be used.
        /// </summary>
        public JObject Config { get; set; }

        /// <summary>
        /// Indicate whether Screenshot is available in the currenct context. Internally, it means GetScreenshot delegate is provided.
        /// </summary>
        public bool CanGetScreenshot => GetScreenshot != EmptyGetScreenshot && GetScreenshot != null;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageReportBuilder"/> class using default options.
        /// </summary>
        public PageReportBuilder()
        {
            Analyze = EmptyAnalyzeDelegate;
            GetScreenshot = EmptyGetScreenshot;
            Options = new PageReportOptions();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageReportBuilder"/> class using the specified options.
        /// </summary>
        /// <param name="options"></param>
        public PageReportBuilder(PageReportOptions options)
        {
            Analyze = EmptyAnalyzeDelegate;
            GetScreenshot = EmptyGetScreenshot;
            Options = options;
        }

        /// <summary>
        /// Provide a custom test options
        /// </summary>
        /// <param name="options">Options</param>
        /// <returns>ReportBuilder with updated options</returns>
        public PageReportBuilder WithOptions(PageReportOptions options)
        {
            Options = options;
            return this;
        }

        /// <summary>
        /// Initilaize axe-core engine with customed RGAA rules.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// This method is not compatible with other <see cref="WithConfig(JObject)"/> method.
        /// </remarks>
        public PageReportBuilder WithRgaaExtension()
        {
            var content = GetRessource("axe-rgaa-extension.json");
            Config = JObject.Parse(content);
            return this;
        }


        /// <summary>
        /// Initilaize axe-core engine with custom configuration.
        /// </summary>
        /// <param name="config">axe costom configuration object</param>
        /// <returns></returns>
        public PageReportBuilder WithConfig(JObject config)
        {
            Config = config;
            return this;
        }


        /// <summary>
        /// Initilaize axe-core engine with customed configuration
        /// </summary>
        /// <param name="configFile">axe core configuration file (in json format)</param>
        /// <returns></returns>
        public PageReportBuilder WithConfig(string configFile)
        {
            var content = File.ReadAllText(configFile);
            Config = JObject.Parse(content);
            return this;
        }

        private byte[] EmptyGetScreenshot(AxeResultNode node, PageReportOptions options)
        {
            return Array.Empty<byte>();
        }


        /// <summary>
        /// Converts the given Axe-Core results to HTML report using actual configuration.
        /// </summary>
        /// <param name="result">The AxeResult to be converted.</param>
        public PageReportBuilder Build(AxeResult result)
        {
            //Add screenshots to AxeResult, then converted to HTML according to the option
            Result = new AxePageResult(result, this);
            return this;
        }

        /// <summary>
        /// Analyze and Build the test report of the given context.
        /// </summary>
        /// <returns></returns>
        public PageReportBuilder Build()
        {
            var config = Config?.DeepClone() ?? new JObject();
            switch(Options.ReportLanguage)
            {
                case Language.French:
                    var localefr = GetRessource("axe-core-fr.json");
                    //add or update the field "locale" in json object config, with the content of fr.json
                    config["locale"] = JObject.Parse(localefr);
                    break;
                case Language.Spanish:
                    var localees = GetRessource("axe-core-es.json");
                    //add or update the field "locale" in json object config, with the content of es.json
                    config["locale"] = JObject.Parse(localees);
                    break;
                case Language.German:
                    var localede = GetRessource("axe-core-de.json");
                    //add or update the field "locale" in json object config, with the content of de.json
                    config["locale"] = JObject.Parse(localede);
                    break;
                case Language.SimplifiedChinese:
                    var localezh = GetRessource("axe-core-zhcn.json");
                    //add or update the field "locale" in json object config, with the content of zhcn.json
                    config["locale"] = JObject.Parse(localezh);
                    break;
                default:
                    break;
            }
            var result = this.Analyze(config as JObject);
            Result = new AxePageResult(result, this);
            return this;
        }

        /// <summary>
        /// Test result of the page.
        /// </summary>
        public AxePageResult Result
        {
            get; private set;
        }

        /// <summary>
        /// Export Enhanced AxeResult (with Screenshots) to expected format.
        /// </summary>
        /// <param name="fileName">The filename of the exported report. Default value is "index.html"</param>
        /// <returns>
        /// absolute path of the exported test report.
        /// </returns>
        public string Export(string fileName = null)
        {
            if (Result == null) throw new InvalidDataException("The report has not been built, please call Build or Analyze before exporting");
            var guid = Guid.NewGuid().ToString();
            string path = Options.OutputFolder ?? Path.Combine(Path.GetTempPath(), guid);
            Directory.CreateDirectory(path);
            string violations = GenerateRuleSection(Result.Violations, path);
            string passes = GenerateRuleSection(Result.Passes, path);
            string incomplete = GenerateRuleSection(Result.Incomplete, path);
            string inapplicable = GenerateRuleSection(Result.Inapplicable, path);
            string html = GetRessource("page-result.html").ToLocale(Options.ReportLanguage);
            html = html.Replace("{{Title}}", Options.Title)
                .Replace("{{PageUrl}}", Result.Url)
                .Replace("{{TimeStamp}}", Result.AxeResult.Timestamp.ToString())
                .Replace("{{Score}}", Result.Score.ToString())
                .Replace("{{ScoreColor}}", Result.ScoreForegroundColor)
                .Replace("{{ScoreBackgroundColor}}", Result.ScoreBackgroundColor)
                .Replace("{{ScoreRotation}}", Result.ScoreRotation.ToString())
                .Replace("{{Violations}}", violations)
                .Replace("{{Passed}}", passes)
                .Replace("{{Incomplete}}", incomplete)
                .Replace("{{ViolationRules}}", Result.Violations.Length.ToString())
                .Replace("{{ViolationNodes}}", Result.Violations.Sum(x => x.Nodes.Length).ToString())
                .Replace("{{NonApplicable}}", inapplicable)
                .Replace("{{IncompleteRules}}", Result.Incomplete.Length.ToString())
                .Replace("{{NonApplicableRules}}", Result.Inapplicable.Length.ToString())
                .Replace("{{PassedRules}}", Result.Passes.Length.ToString());

            string fullname = Path.Combine(path, fileName ?? "index.html");
            File.WriteAllText(fullname, html);

            switch (Options.OutputFormat)
            {
                case OutputFormat.Html:
                    return fullname;
                case OutputFormat.Zip:
                    var file = Path.GetRandomFileName();
                    var zipName = Path.Combine(path, "report.zip");
                    File.Delete(file);
                    ZipFile.CreateFromDirectory(path, file);
                    Directory.Delete(path, true);
                    Directory.CreateDirectory(path);
                    File.Copy(file, zipName, true);
                    File.Delete(file);
                    return zipName;
                default:
                    // it will be a bug if new output format is not yet implemented.
                    throw new NotImplementedException($"Output format is not yet supported {Options.OutputFormat}");
            }
        }

        internal static string GetRessource(string filename)
        {
            //read content from Embeded Resource `Assets/index.html`
            var assembly = typeof(PageReportBuilder).Assembly;
            var resourceName = assembly.GetName().Name + ".Assets." + filename;
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    throw new FileNotFoundException($"Unable to find resource {resourceName} in assembly {assembly.FullName}");
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        static int uniqueCheckId = 0;
        static int UniqueCheckId
        {
            get
            {
                return uniqueCheckId++;
            }
        }

        private string GenerateRuleSection(AxeResultEnhancedItem[] items, string path)
        {

            StringBuilder overall = new StringBuilder();
            var template = GetRessource("rule-part.html").ToLocale(Options.ReportLanguage);
            foreach (var item in items)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var node in item.Nodes)
                {
                    //generate node (occurances of rule)
                    var nodeTemplate = GetRessource("node-part.html").ToLocale(Options.ReportLanguage);
                    var cssSelector = node.Node.Target;
                    var display = node.Screenshot != null ? "block" : "none";
                    string filename = string.Empty;
                    if (node.Screenshot != null)
                    {
                        Guid id = Guid.NewGuid();
                        filename = id.ToString() + ".png";
                        File.WriteAllBytes(Path.Combine(path, filename), node.Screenshot);
                    }

                    sb.Append(
                        nodeTemplate.Replace("{{Selector}}", cssSelector.ToString())
                        .Replace("{{HtmlCode}}", HttpUtility.HtmlEncode(node.Node.Html))
                        .Replace("{{Display}}", display)
                        .Replace("{{Filename}}", filename)
                        .Replace("{{UniqueCheckId}}", UniqueCheckId.ToString())
                        .Replace("{{AnyChecks}}", GenerateChecksSection(node.Node.Any, "Any Checks"))
                        .Replace("{{AllChecks}}", GenerateChecksSection(node.Node.All, "All Checks"))
                        .Replace("{{NoneChecks}}", GenerateChecksSection(node.Node.None, "None Checks"))
                        );
                }
                string tags = string.Join(" ", item.Item.Tags.Select(x => $"<div class='regularition'>{x}</div>"));
                var additinalTags = Options.AdditionalTags?.GetTagsByRule(item.Item);
                if (additinalTags != null)
                {
                    tags += string.Join(" ", additinalTags.Select(x => $"<div class='regularition'>{x}</div>"));
                }
                overall.Append(
                    template.Replace("{{RuleId}}", item.Item.Id)
                    .Replace("{{RuleTags}}", tags)
                    .Replace("{{Impact}}", item.Item.GetImpact())
                    .Replace("{{Description}}", HttpUtility.HtmlEncode(item.Item.Description))
                    .Replace("{{Help}}", HttpUtility.HtmlEncode(item.Item.Help))
                    .Replace("{{HelpUrl}}", item.Item.HelpUrl)
                    .Replace("{{RuleNodeCount}}", item.Nodes.Length.ToString())
                    .Replace("{{RuleNodes}}", sb.ToString()));
            }

            
            

            return overall.ToString();
        }

        private string GenerateChecksSection(AxeResultCheck[] any, string label)
        {
            if (any != null && any.Any())
            {
                var sb = new StringBuilder();
                foreach (AxeResultCheck item in any)
                {
                    var template = GetRessource("check-part.html").ToLocale(Options.ReportLanguage)
                        .Replace("{{CheckId}}", item.Id)
                        .Replace("{{Impact}}", item.Impact)
                        .Replace("{{Message}}", WebUtility.HtmlEncode(item.Message))
                        .Replace("{{HasData}}", item.Data != null ? "block" : "none")
                        .Replace("{{Data}}", WebUtility.HtmlEncode(item.Data?.ToString() ?? string.Empty));
                    sb.AppendLine(template);
                }
                return sb.ToString();
            }
            return "No rules have audited for " + label;
        }

        /// <summary>
        /// Delegate to get screenshot of the given node. this function should be implemented according to test framework. such as Selenium.
        /// </summary>
        public GetScreenshotDelegate GetScreenshot { get; internal set; }

        private JObject locale = null;
        
        /// <summary>
        /// Delegate to analyze the given context. this function should be implemented according to test framework. such as using Selenium.
        /// </summary>
        public AnalyzeDelegate Analyze { get; internal set; }

        /// <summary>
        /// Delegate to get screenshot of the given node. this function should be implemented according to test framework. such as Selenium.
        /// </summary>

        public delegate byte[] GetScreenshotDelegate(AxeResultNode node, PageReportOptions options);

        /// <summary>
        /// Delegate to analyze the given context. this function should be implemented according to test framework. such as using Selenium.
        /// </summary>
        public delegate AxeResult AnalyzeDelegate(JObject axeConfig);


        /// <summary>
        /// This is the default Emoty delegate for Analyze. without calling WithSelenium or WithPlaywright, analyze will be empty.
        /// </summary>
        /// <returns></returns>
        private AxeResult EmptyAnalyzeDelegate(JObject config)
        {
            throw new System.NotImplementedException("Test Framework Not specified. Please pass .WithSelenium(driver) or .WithPlaywright(context) before calling analysis.");
        }

    }
}