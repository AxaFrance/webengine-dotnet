using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axe.Extended.HtmlReport
{
    /// <summary>
    /// Options of generating HTML report
    /// </summary>
    public sealed class PageReportOptions
    {
        /// <summary>
        /// Gets or sets the output format of the HTML report: Zip Archive or a folder containing HTML report and all resources.
        /// </summary>
        public OutputFormat OutputFormat { get; set; } = OutputFormat.Html;

        /// <summary>
        /// Gets or sets the output folder of the HTML report. If not set, the report will be generated in default temprary folder.
        /// </summary>
        public string OutputFolder { get; set; }

        /// <summary>
        /// The language should be used in the report. Default is English.
        /// </summary>
        public Language ReportLanguage { get; set; } = Language.English;

        /// <summary>
        /// In the report, show the RGAA tags (equivalent RGAA Test Methdologie) for each rule. Default is true.
        /// Default value: will use additional tags provider to get equivalent RGAA test tags.
        /// </summary>
        public AdditionalTagsProvider AdditionalTags { get; set; } = new RgaaTagsProvider();

        /// <summary>
        /// Gets or sets the value indicating if advanced screenshot should be used. Default is true.
        /// </summary>
        /// <remarks>
        /// Advanced screenshot is a screenshot of the whole viewport, and the element in question is highlighted.
        /// Otherwise, a screenshot of the element in question is taken.
        /// Advanced screenshot marks the element in question more clearly, but it takes more space (images are larger).
        /// </remarks>
        public bool UseAdvancedScreenshot { get; set; } = true;

        /// <summary>
        /// Default highlight color for advanced screenshot. default value is Red.
        /// </summary>
        public Color HighlightColor { get; set; } = Color.Red;

        /// <summary>
        /// Default highlight thickness for advanced screenshot. default value is 2.
        /// </summary>
        public int HighlightThickness { get; set; } = 2;


        /// <summary>
        /// If true, the report will include screenshots for all violations. Default is true.
        /// </summary>
        public bool ScreenshotViolations { get; set; } = true;

        /// <summary>
        /// If true, the report will include screenshots for all passes. Default is false.
        /// </summary>
        public bool ScreenshotPasses { get; set; } = false;

        /// <summary>
        /// If true, the report will include screenshots for all incomplete. Default is true.
        /// </summary>
        public bool ScreenshotIncomplete { get; set; } = true;

        /// <summary>
        /// The mode to calculate the accessbility scoring, default method = <see cref="ScoringMode.Weighted"/>
        /// </summary>
        public ScoringMode ScoringMode { get; set; } = ScoringMode.Weighted;
        
        /// <summary>
        /// Title of the report
        /// </summary>
        public string Title { get; set; } = "Accessibility Report";

        /// <summary>
        /// Tags used to be audited by the tool. Default Value: [empty], all rules will be tested.
        /// To get the complete list of all tags, refer to: https://www.deque.com/axe/core-documentation/api-documentation/#api-notes
        /// Leaving Tag empty means all rules will be tested.
        /// </summary>
        public IEnumerable<string> Tags { get; set; } = new string[] { };



        /// <summary>
        /// A and AA rules defined in WCAG 2.0, 2.1 and 2.2 and EN-301-549
        /// </summary>
        public static readonly string[] WcagAATags = new string[]
        {
            "wcag2a", "wcag2aa", "wcag2aaa", "wcag21a", "wcag21aa", "wcag22aa", "EN-301-549", "wcag***"
        };

        internal PageReportOptions Clone()
        {
            return new PageReportOptions()
            {
                OutputFormat = OutputFormat,
                OutputFolder = OutputFolder,
                ReportLanguage = ReportLanguage,
                HighlightColor = HighlightColor,
                ScreenshotViolations = ScreenshotViolations,
                ScreenshotPasses = ScreenshotPasses,
                ScreenshotIncomplete = ScreenshotIncomplete,
                HighlightThickness = HighlightThickness,
                ScoringMode = ScoringMode,
                Tags = Tags,
                Title = Title,
                UseAdvancedScreenshot = UseAdvancedScreenshot,
            };
        }
    }
}
