using Deque.AxeCore.Commons;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AxaFrance.AxeExtended.HtmlReport
{
    /// <summary>
    /// Extension methods for AxeResultItem
    /// </summary>
    internal static class ExtensionMethods
    {

        /// <summary>
        /// Get the impact of the AxeResultItem.
        /// </summary>
        /// <param name="item"><see cref="AxeResultItem"/> object</param>
        /// <returns>The impact value</returns>
        public static string GetImpact(this AxeResultItem item)
        {
            
            var impact = item.Impact;
            if (impact == null)
            {
                impact = item.Nodes.FirstOrDefault()?.Impact;
            }
            if(impact == null)
            {
                impact = item.Nodes.FirstOrDefault()?.All.FirstOrDefault()?.Impact;
            }
            if (impact == null)
            {
                impact = item.Nodes.FirstOrDefault()?.Any.FirstOrDefault()?.Impact;
            }
            if (impact == null)
            {
                impact = item.Nodes.FirstOrDefault()?.None.FirstOrDefault()?.Impact;
            }
            return impact;
        }

        static Dictionary<Language, JObject> locales
            = new Dictionary<Language, JObject>();

        internal static string ToLocale(this string str, Language lang)
        {
            if (!locales.ContainsKey(lang))
            {
                string locale;
                switch (lang)
                {
                    case Language.French:
                        locale = PageReportBuilder.GetRessource("report-fr.json");
                        break;
                    case Language.Spanish:
                        locale = PageReportBuilder.GetRessource("report-es.json");
                        break;
                    case Language.German:
                        locale = PageReportBuilder.GetRessource("report-de.json");
                        break;
                    case Language.SimplifiedChinese:
                        locale = PageReportBuilder.GetRessource("report-zhcn.json");
                        break;
                    default:
                        locale = PageReportBuilder.GetRessource("report-en.json");
                        break;

                }
                locales[lang] = JObject.Parse(locale);
            }
            //replace all occurences of "${key}" by the value in the locale
            var sb = new StringBuilder(str);
            foreach (var key in locales[lang].Properties())
            {
                sb.Replace("${" + key.Name + "}", locales[lang][key.Name].ToString());
            }
            return sb.ToString();
        }

    }
}
