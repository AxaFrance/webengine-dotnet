using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axe.Extended.HtmlReport
{

    /// <summary>
    /// A mapping of Axe rules to RGAA rules, for future use
    /// refers to the Mapping Rules Excel file
    /// </summary>
    internal class RgaaTagsProvider : AdditionalTagsProvider
    {

        private static Dictionary<string, IEnumerable<string>> Mapping { get; } = new Dictionary<string, IEnumerable<string>>()
        {
            {"area-alt", new string[] {"1.1.2"}},
            {"aria-hidden-focus", new string[] {"10.8.1"}},
            {"aria-input-field-name", new string[] {"11.1.1"}},
            {"blink", new string[] {"10.1.1"}},
            {"button-name", new string[] {"11.9.1", "11.1.1", "11.2.6"}},
            {"color-contrast", new string[] {"3.2.1", "3.2.2", "3.2.3", "3.2.4"}},
            {"definition-list", new string[] {"9.3.3"}},
            {"dlitem", new string[] {"9.3.3"}},
            {"document-title", new string[] {"8.5.1"}},
            {"frame-title-unique", new string[] {"2.1.1"}},
            {"frame-title", new string[] {"2.2.1"}},
            {"html-has-lang", new string[] {"8.3.1"}},
            {"html-lang-valid", new string[] {"8.3.1", "8.4.1"}},
            {"html-xml-lang-mismatch", new string[] {"8.7.1", "8.8.1"}},
            {"image-alt", new string[] {"1.1.1"}},
            {"input-button-name", new string[] {"11.9.1", "11.9.2"}},
            {"input-image-alt", new string[] {"1.1.3"}},
            {"label", new string[] {"11.1.1", "11.1.2"}},
            {"link-in-text-block", new string[] {"10.6.1"}},
            {"link-name", new string[] {"6.1.1"}},
            {"list", new string[] {"9.3.1", "9.3.2"}},
            {"listitem", new string[] {"9.3.1", "9.3.2"}},
            {"marquee", new string[] {"10.1.1"}},
            {"meta-refresh", new string[] {"13.3.1"}},
            {"no-autoplay-audio", new string[] {"4.10.1"}},
            {"object-alt", new string[] {"1.1.6"}},
            {"role-img-alt", new string[] {"1.1.1"}},
            {"select-name", new string[] {"11.1.1", "11.1.2"}},
            {"svg-img-alt", new string[] {"1.1.5"}},
            {"td-headers-attr", new string[] {"5.6.1", "5.6.2", "5.7.1"}},
            {"th-has-data-cells", new string[] {"5.7.2"}},
            {"valid-lang", new string[] {"8.3.1", "8.4.1", "8.8.1"}},
            {"video-caption", new string[] {"4.1.2", "4.1.3"}},
            {"autocomplete-valid", new string[] {"11.13.1"}},
            {"empty-heading", new string[] {"9.1.2"}},
            {"heading-order", new string[] {"9.1.1", "9.1.3"}},
            {"landmark-banner-is-top-level", new string[] {"9.2.1"}},
            {"landmark-complementary-is-top-level", new string[] {"9.2.1"}},
            {"landmark-contentinfo-is-top-level", new string[] {"9.2.1"}},
            {"landmark-main-is-top-level", new string[] {"9.2.1"}},
            {"landmark-no-duplicate-banner", new string[] {"9.2.1"}},
            {"landmark-no-duplicate-contentinfo", new string[] {"9.2.1"}},
            {"landmark-no-duplicate-main", new string[] {"9.2.1"}},
            {"landmark-one-main", new string[] {"9.2.1"}},
            {"landmark-unique", new string[] {"9.2.1"}},
            {"meta-viewport-large", new string[] {"13.9.1"}},
            {"region", new string[] {"9.2.1"}},
            {"scope-attr-valid", new string[] {"5.7.2"}},
            {"tabindex", new string[] {"12.8.1"}},
            {"table-duplicate-name", new string[] {"15.2.1"}},
            {"meta-refresh-no-exceptions", new string[] {"13.1.2"}},
            {"css-orientation-lock", new string[] {"13.9.1"}},
            {"hidden-content", new string[] {"10.8.1"}},
            {"label-content-name-mismatch", new string[] {"11.2.3", "11.2.4"}},
            {"table-fake-caption", new string[] {"5.1.1"}},
        };

        public override IEnumerable<string> GetTagsByRule(string ruleId)
        {

            if (Mapping.ContainsKey(ruleId))
            {
                return Mapping[ruleId];
            }
            else
            {
                return Array.Empty<string>();
            }
        }
    }
}
