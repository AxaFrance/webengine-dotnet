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
        public PageReportOptions Options { get; set; }


        /// <summary>
        /// Provide a custom axe configure (https://github.com/dequelabs/axe-core/blob/master/doc/API.md#api-name-axeconfigure).
        /// If provided, the configuration will be used. If not provided, the default axe configuration will be used.
        /// </summary>
        public JObject Config { get; set; }

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

        public PageReportBuilder WithOptions(PageReportOptions options)
        {
            Options = options;
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
        /// Initilaize axe-core engine with customed RGAA rules.
        /// </summary>
        /// <param name="config"></param>
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

        public PageReportBuilder WithConfig(string configFile)
        {
            var content = File.ReadAllText(configFile);
            Config = JObject.Parse(content);
            return this;
        }

        private byte[] EmptyGetScreenshot(AxeResultNode node, PageReportOptions options)
        {
            return null;
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
            var result = this.Analyze(Config);
            Result = new AxePageResult(result, this);
            return this;
        }

        public AxePageResult Result
        {
            get; private set;
        }

        /// <summary>
        /// Export Enhanced AxeResult (with Screenshots) to expected format.
        /// </summary>
        /// <param name="result">Processed AxeResult with screenshot</param>
        /// <returns>
        /// absolute path of the exported test report.
        /// </returns>
        public string Export(string fileName = null)
        {
            if (Result == null) throw new InvalidDataException("The report has not been built, please call Build or Analyze before exporting");
            var guid = Guid.NewGuid().ToString();
            string path = Options.OutputFolder ?? Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(path);
            string violations = GenerateRuleSection(Result.Violations, path);
            string passes = GenerateRuleSection(Result.Passes, path);
            string incomplete = GenerateRuleSection(Result.Incomplete, path);
            string inapplicable = GenerateRuleSection(Result.Inapplicable, path);
            string html = GetRessource("page-result.html");
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
                .Replace("{{ViolationRules}}", Result.Violations.Count().ToString())
                .Replace("{{ViolationNodes}}", Result.Violations.Sum(x => x.Nodes.Count()).ToString())
                .Replace("{{NonApplicable}}", inapplicable)
                .Replace("{{IncompleteRules}}", Result.Incomplete.Count().ToString())
                .Replace("{{NonApplicableRules}}", Result.Inapplicable.Count().ToString())
                .Replace("{{PassedRules}}", Result.Passes.Count().ToString());

            string fullname = Path.Combine(path, fileName ?? "index.html");
            File.WriteAllText(fullname, html);

            switch (Options.OutputFormat)
            {
                case OutputFormat.Html:
                    return fullname;
                case OutputFormat.Zip:
                    var file = Path.GetTempFileName();
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
                    throw new Exception($"Unable to find resource {resourceName} in assembly {assembly.FullName}");
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
            var template = GetRessource("rule-part.html");
            foreach (var item in items)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var node in item.Nodes)
                {
                    //generate node (occurances of rule)
                    var nodeTemplate = GetRessource("node-part.html");
                    var cssSelector = node.Node.Target;
                    var xpath = node.Node.XPath;
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
                var rgaaTags = Options.AdditionalTags?.GetTagsByRule(item.Item.Id);
                if (rgaaTags != null)
                {
                    tags += string.Join(" ", rgaaTags.Select(x => $"<div class='regularition'>RGAA {x}</div>"));
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
                    var template = GetRessource("check-part.html")
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

        public GetScreenshotDelegate GetScreenshot { get; internal set; }
        public AnalyzeDelegate Analyze { get; internal set; }

        public delegate byte[] GetScreenshotDelegate(AxeResultNode node, PageReportOptions options);
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