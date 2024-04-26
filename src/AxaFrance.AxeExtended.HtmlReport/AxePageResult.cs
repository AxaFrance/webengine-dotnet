using Deque.AxeCore.Commons;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axe.Extended.HtmlReport
{
    /// <summary>
    /// Represents the complete result of accessibility test for a single page.
    /// </summary>
    /// <remarks>
    /// On a complete audit of an application, the test should be performed on all pages.
    /// </remarks>
    public class AxePageResult: BaseResult
    {

        /// <summary>
        /// Underlying AxeResult
        /// </summary>
        public AxeResult AxeResult { get; private set; }

        public PageReportBuilder Builder { get; private set; }

        public AxePageResult(AxeResult result, PageReportBuilder htmlReportBuilder)
        {
            Builder = htmlReportBuilder;
            AxeResult = result;
            Violations = GetViolations(result, htmlReportBuilder);
            Passes = GetPasses(result, htmlReportBuilder);
            Incomplete = GetIncomplete(result, htmlReportBuilder);
            Inapplicable = GetInapplicable(result, htmlReportBuilder);
            TestEngine = result.TestEngine;
            TestRunner = result.TestRunner;
            Timestamp = result.Timestamp;
            Url = result.Url;
            TestEnvironment = result.TestEnvironment;
        }

        private AxeResultEnhancedItem[] GetInapplicable(AxeResult result, PageReportBuilder htmlReportBuilder)
        {
            List<AxeResultEnhancedItem> inapplicable = new List<AxeResultEnhancedItem>();
            foreach (var i in result.Inapplicable)
            {
                AxeResultEnhancedItem item = new AxeResultEnhancedItem(i, GetEnhancedNodes(i.Nodes, htmlReportBuilder, false));
                inapplicable.Add(item);
            }
            return inapplicable.ToArray();
        }

        private AxeResultEnhancedItem[] GetIncomplete(AxeResult result, PageReportBuilder htmlReportBuilder)
        {
            List<AxeResultEnhancedItem> incomplete = new List<AxeResultEnhancedItem>();
            bool takeScreenshot = htmlReportBuilder.Options.ScreenshotIncomplete;
            foreach (var i in result.Incomplete)
            {
                AxeResultEnhancedItem item = new AxeResultEnhancedItem(i, GetEnhancedNodes(i.Nodes, htmlReportBuilder, takeScreenshot));
                incomplete.Add(item);
            }
            return incomplete.ToArray();
        }

        private AxeResultEnhancedItem[] GetPasses(AxeResult result, PageReportBuilder htmlReportBuilder)
        {
            List<AxeResultEnhancedItem> passes = new List<AxeResultEnhancedItem>();
            bool takeScreenshot = htmlReportBuilder.Options.ScreenshotPasses;
            foreach (var i in result.Passes)
            {
                AxeResultEnhancedItem item = new AxeResultEnhancedItem(i, GetEnhancedNodes(i.Nodes, htmlReportBuilder, takeScreenshot));
                passes.Add(item);
            }
            return passes.ToArray();
        }

        private AxeResultEnhancedItem[] GetViolations(AxeResult result, PageReportBuilder htmlReportBuilder)
        {
            List<AxeResultEnhancedItem> violations = new List<AxeResultEnhancedItem>();
            bool takeScreenshot = htmlReportBuilder.Options.ScreenshotViolations;
            foreach (var i in result.Violations)
            {
                AxeResultEnhancedItem item = new AxeResultEnhancedItem(i, GetEnhancedNodes(i.Nodes, htmlReportBuilder, takeScreenshot));
                violations.Add(item);
            }

            return violations.ToArray();
        }

        private AxeResultEnhancedNode[] GetEnhancedNodes(AxeResultNode[] nodes, PageReportBuilder htmlReportBuilder, bool takeScreenshot)
        {
            List<AxeResultEnhancedNode> axeResultNodeEnhanceds = new List<AxeResultEnhancedNode>();
            foreach (var n in nodes)
            {
                AxeResultEnhancedNode axeResultNodeEnhanced = new AxeResultEnhancedNode(n);
                if (takeScreenshot)
                {
                    axeResultNodeEnhanced.Screenshot = htmlReportBuilder.GetScreenshot(n, htmlReportBuilder.Options);
                }
                axeResultNodeEnhanceds.Add(axeResultNodeEnhanced);
            }
            return axeResultNodeEnhanceds.ToArray();
        }


        /// <summary>
        /// Calculate the score of the tested page using following weighted methods for Failed and Passed audits.
        /// </summary>
        /// <returns>The accessibility score.</returns>
        /// <remarks>
        /// * Weight of each passed and failed audit is based on impact of each axe rule: critical, seruous, moderate or minor
        /// * Incomplete rules are not calculated in the score
        /// </remarks>
        protected override int GetScore()
        {
            int violationScore = 0;
            int passeScore = 0;
            var mode = Builder.Options.ScoringMode;

            
            foreach (var violation in this.Violations)
            {
                switch (mode)
                {
                    case ScoringMode.Weighted:
                        violationScore += ScorePerImpact(violation.Item);
                        break;
                    case ScoringMode.NonWeighted:
                        violationScore += 1;
                        break;
                    case ScoringMode.WeightedOccurence:
                        violationScore += ScorePerImpact(violation.Item) * violation.Nodes.Length; 
                        break;
                }
            }
            foreach (var passed in this.Passes)
            {
                if(this.Violations.FirstOrDefault(x=>x.Item.Id == passed.Item.Id) != null)
                {
                    //In this case, there are elements passed but other elements faied the.
                    //we don't count check passed
                    continue;
                }

                switch (mode)
                {
                    case ScoringMode.Weighted:
                        passeScore += ScorePerImpact(passed.Item);
                        break;
                    case ScoringMode.NonWeighted:
                        passeScore += 1;
                        break;
                    case ScoringMode.WeightedOccurence:
                        passeScore += ScorePerImpact(passed.Item) * passed.Nodes.Length;
                        break;
                }
            }
            Scorebase = passeScore + violationScore;
            if(Scorebase == 0)
            {
                return 0;
            }
            int score = passeScore * 100 / Scorebase;
            Score = score;
            ScoreRotation = Score * 360 / 100;
            return score;
        }


        /// <summary>
        /// Get the weight according to the impact, uses the same weighting score as lighthouse
        /// </summary>
        /// <param name="impact">Acessibility impact: Critical, Serious, Moderate, Minor</param>
        /// <returns>Weight: 1, 3, 7, and 10 according to impact</returns>
        internal static int ScorePerImpact(AxeResultItem resultItem)
        {
            var impact = resultItem.GetImpact();

            if (impact == null)
            {
                //if there is no impact, we consider it as moderate
                return 3;
            }
            switch (impact.ToLower())
            {
                case "critical":
                    return 10;
                case "serious":
                    return 7;
                case "moderate":
                    return 3;
                case "minor":
                    return 1;
                default:
                    throw new ArgumentOutOfRangeException($"{impact} is not an expected impact.");

            }
        }

        /// <summary>
        /// These results indicate what elements failed the rules.
        /// </summary>
        public AxeResultEnhancedItem[] Violations { get; internal set; }

        /// <summary>
        /// These results indicate what elements passed the rules.
        /// </summary>
        public AxeResultEnhancedItem[] Passes { get; internal set; }

        /// <summary>
        /// These results indicate which rules did not run because no matching content was found on the page. 
        /// For example, with no video, those rules won't run.
        /// </summary>
        public AxeResultEnhancedItem[] Inapplicable { get; internal set; }

        /// <summary>
        /// These results were aborted and require further testing. 
        /// This can happen either because of technical restrictions to what the rule can test, or because a javascript error occurred.
        /// </summary>
        public AxeResultEnhancedItem[] Incomplete { get; internal set; }

        /// <summary>
        /// The date and time that analysis was completed.
        /// </summary>
        public DateTimeOffset? Timestamp { get; private set; }

        /// <summary>
        /// Information about the current browser or node application that ran the audit.
        /// </summary>
        public AxeTestEnvironment TestEnvironment { get; private set; }

        /// <summary>
        /// The runner that ran the audit.
        /// </summary>
        public AxeTestRunner TestRunner { get; set; }

        /// <summary>
        /// The URL of the page that was tested.
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        /// The application that ran the audit.
        /// </summary>
        public AxeTestEngine TestEngine { get; private set; }

    }
}
