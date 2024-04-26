using Deque.AxeCore.Commons;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axe.Extended.HtmlReport
{
    public class OverallReportBuilder
    {
        private PageReportOptions options;

        public OverallReportBuilder() {
            options = new PageReportOptions();
            PageBuilders = new List<PageReportBuilder>();
        }

        public OverallReportBuilder(PageReportOptions options)
        {
            this.options = options;
            PageBuilders = new List<PageReportBuilder>();
        }

        public OverallReportBuilder WithDefaultOptions(PageReportOptions options)
        {
            this.options = options;
            return this;
        }

        /// <summary>
        /// Build overall report from scanned pages which are built by PageReportBuilder.
        /// </summary>
        /// <returns></returns>
        public OverallReportBuilder Build()
        {
            AxeOverallResult overallResult = new AxeOverallResult()
            {
                Title = options.Title,
                TimeStamp = DateTime.Now
            };
            foreach(var pageBuilder in PageBuilders)
            {
                if (pageBuilder.Result != null)
                {
                    overallResult.PageResults.Add(pageBuilder.Result);
                }
            }
            this.Result = overallResult;
            hasBuilt = true;
            return this;
        }

        bool hasBuilt = false;

        /// <summary>
        /// Export overall report to HTML or Zip format.
        /// </summary>
        /// <param name="fileName">the filename of export report, default value "index.html"</param>
        /// <returns>the complete path of report</returns>
        /// <exception cref="NotImplementedException"></exception>
        public string Export(string fileName = null)
        {
            if (!hasBuilt) Build();
            string path = Options.OutputFolder ?? Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            
            //clean the folder
            Directory.Delete(path, true);
            Directory.CreateDirectory(path);
            int sequence = 1;

            //export the overallpage
            var html = PageReportBuilder.GetRessource("overall-result.html");
            var rowTemplate = PageReportBuilder.GetRessource("overall-tablerow.html");
            var TableRowPageResult = new StringBuilder();
            
            //export each subpages and generation of 
            foreach (var pageBuilder in PageBuilders)
            {
                if (pageBuilder.Result != null)
                {
                    string subFolder = "page" + sequence++;
                    string filename = "page-report.html";
                    pageBuilder.Options.OutputFolder = Path.Combine(path, subFolder);
                    pageBuilder.Options.OutputFormat = OutputFormat.Html;
                    pageBuilder.Export(filename);
                    TableRowPageResult.AppendLine(rowTemplate.Replace("{{PageTitle}}", pageBuilder.Options.Title)
                        .Replace("{{PageUrl}}", pageBuilder.Result.Url)
                        .Replace("{{Score}}", pageBuilder.Result.Score.ToString())
                        .Replace("{{ScoreColor}}", pageBuilder.Result.ScoreForegroundColor)
                        .Replace("{{ReportLink}}", subFolder + "/" + filename));
                }
            }

            html = html.Replace("{{Title}}", Options.Title)
                    .Replace("{{TimeStamp}}", this.Result.TimeStamp.ToString())
                    .Replace("{{Score}}", this.Result.Score.ToString())
                    .Replace("{{TableRowPageResult}}", TableRowPageResult.ToString())
                    .Replace("{{ScoreBackgroundColor}}", this.Result.ScoreBackgroundColor)
                    .Replace("{{ScoreColor}}", this.Result.ScoreForegroundColor)
                    .Replace("{{ScoreRotation}}", this.Result.ScoreRotation.ToString());


            StringBuilder ruleTitles= new StringBuilder();
            StringBuilder ruleResults = new StringBuilder();

            //The header of the overall view table
            ruleTitles.AppendLine("<th>Rule Id</th>");
            ruleTitles.AppendLine("<th>Tags</th>");
            foreach(var page in Result.PageResults)
            {
                ruleTitles.AppendLine($"<th>{page.Builder.Options.Title}</th>");               
            }
            ruleTitles.AppendLine($"<th>Overall</th>");

            //The content of the overall view table
            foreach (var rule in Result.overallResults.OrderBy(x=>x.Key))
            {
                var ruleId = rule.Key;
                var rResult = rule.Value;
                ruleResults.AppendLine($"<tr><td>{ruleId}</td>");
                
                //Add tags
                ruleResults.AppendLine($"<td>");
                var rgaaTags = options.AdditionalTags?.GetTagsByRule(ruleId);
                if (rgaaTags != null)
                {
                    foreach (var tag in rgaaTags)
                    {
                        ruleResults.AppendLine($"<span class='tag'>RGAA {tag}</span>");
                    }
                }
                ruleResults.AppendLine($"</td>");
                foreach (var page in Result.PageResults)
                {
                    if (page.Violations.FirstOrDefault(x => x.Item.Id == ruleId) != null)
                    {
                        ruleResults.AppendLine($"<td><span class='Violations'>Violations</span></td>");
                    }
                    else if (page.Incomplete.FirstOrDefault(x => x.Item.Id == ruleId) != null)
                    {
                        ruleResults.AppendLine($"<td><span class='Incomplele'>Incomplele</span></td>");
                    }
                    else if (page.Passes.FirstOrDefault(x=>x.Item.Id == ruleId) != null)
                    {
                        ruleResults.AppendLine($"<td><span class='Passes'>Passes</span></td>");
                    }
                    else
                    {
                        ruleResults.AppendLine($"<td><span class='Inapplicable'>Inapplicable</span></td>");
                    }
                }
                ruleResults.AppendLine($"<td><span class='{rResult}'>{rResult}</span></td>");
                ruleResults.AppendLine("</tr>");
            }


            html = html.Replace("{{RowPageHeaderList}}", ruleTitles.ToString())
                .Replace("{{RowRuleResults}}", ruleResults.ToString());
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

        public PageReportOptions Options
        {
            get { return this.options; }
        }

        public List<PageReportBuilder> PageBuilders { get; internal set; }
        public AxeOverallResult Result { get; private set; }
    }
}
