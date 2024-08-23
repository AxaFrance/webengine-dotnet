using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AxaFrance.AxeExtended.HtmlReport
{
    /// <summary>
    /// Language to be used for report. Placeholder.
    /// </summary>
    public enum Language
    {
        /// <summary>
        /// English
        /// </summary>
        English,

        /// <summary>
        /// French
        /// </summary>
        French,

        /// <summary>
        /// Spanish
        /// </summary>
        Spanish,

        /// <summary>
        /// German
        /// </summary>
        [Obsolete("German language is not not working due to axe core ressource issues.")]
        German,

        /// <summary>
        /// Simplified Chinese
        /// </summary>
        SimplifiedChinese

        //Remark: Other languages, necearry to translate role descriptions into these languages.
        //Assets/axe-core-xx.json -> from https://github.com/dequelabs/axe-core/tree/develop/locales
        //Assets/report-xx.json -> translate from report-en.json
        //Modify ExtensionMethods.ToLocale() to add new language in generated report
        //Modify PageReportBuilder.Build() to add new language in axe core scan
    }
}
