using Deque.AxeCore.Commons;
using System;
using System.Collections.Generic;

namespace AxaFrance.AxeExtended.HtmlReport
{

    /// <summary>
    /// A mapping of Axe rules to RGAA rules, for future use
    /// refers to the Mapping Rules Excel file
    /// </summary>
    public class RgaaTagsProvider : TagsProvider
    {

        private static Dictionary<string, IEnumerable<string>> Mapping { get; } = new Dictionary<string, IEnumerable<string>>()
        {
            {"area-alt", new string[] {"RGAA 1.1.2"}},
            {"aria-hidden-focus", new string[] {"RGAA 10.8.1"}},
            {"aria-input-field-name", new string[] {"RGAA 11.1.1"}},
            {"blink", new string[] {"RGAA 10.1.1"}},
            {"button-name", new string[] { "RGAA 11.9.1", "RGAA 11.1.1", "RGAA 11.2.6"}},
            {"color-contrast", new string[] { "RGAA 3.2.1", "RGAA 3.2.2", "RGAA 3.2.3", "RGAA 3.2.4"}},
            {"definition-list", new string[] {"RGAA 9.3.3"}},
            {"dlitem", new string[] {"RGAA 9.3.3"}},
            {"document-title", new string[] {"RGAA 8.5.1"}},
            {"frame-title-unique", new string[] {"RGAA 2.1.1"}},
            {"frame-title", new string[] {"RGAA 2.2.1"}},
            {"html-has-lang", new string[] {"RGAA 8.3.1"}},
            {"html-lang-valid", new string[] { "RGAA 8.3.1", "RGAA 8.4.1"}},
            {"html-xml-lang-mismatch", new string[] { "RGAA 8.7.1", "RGAA 8.8.1"}},
            {"image-alt", new string[] {"RGAA 1.1.1"}},
            {"input-button-name", new string[] { "RGAA 11.9.1", "RGAA 11.9.2"}},
            {"input-image-alt", new string[] {"RGAA 1.1.3"}},
            {"label", new string[] { "RGAA 11.1.1", "RGAA 11.1.2"}},
            {"link-in-text-block", new string[] {"RGAA 10.6.1"}},
            {"link-name", new string[] {"RGAA 6.1.1"}},
            {"list", new string[] { "RGAA 9.3.1", "RGAA 9.3.2"}},
            {"listitem", new string[] { "RGAA 9.3.1", "RGAA 9.3.2"}},
            {"marquee", new string[] {"RGAA 10.1.1"}},
            {"meta-refresh", new string[] {"RGAA 13.3.1"}},
            {"no-autoplay-audio", new string[] {"RGAA 4.10.1"}},
            {"object-alt", new string[] {"RGAA 1.1.6"}},
            {"role-img-alt", new string[] {"RGAA 1.1.1"}},
            {"select-name", new string[] { "RGAA 11.1.1", "RGAA 11.1.2"}},
            {"svg-img-alt", new string[] {"RGAA 1.1.5"}},
            {"td-headers-attr", new string[] { "RGAA 5.6.1", "RGAA 5.6.2", "RGAA 5.7.1"}},
            {"th-has-data-cells", new string[] {"RGAA 5.7.2"}},
            {"valid-lang", new string[] { "RGAA 8.3.1", "RGAA 8.4.1", "RGAA 8.8.1"}},
            {"video-caption", new string[] { "RGAA 4.1.2", "RGAA 4.1.3"}},
            {"autocomplete-valid", new string[] {"RGAA 11.13.1"}},
            {"empty-heading", new string[] {"RGAA 9.1.2"}},
            {"heading-order", new string[] { "RGAA 9.1.1", "RGAA 9.1.3"}},
            {"landmark-banner-is-top-level", new string[] {"RGAA 9.2.1"}},
            {"landmark-complementary-is-top-level", new string[] {"RGAA 9.2.1"}},
            {"landmark-contentinfo-is-top-level", new string[] {"RGAA 9.2.1"}},
            {"landmark-main-is-top-level", new string[] {"RGAA 9.2.1" }},
            {"landmark-no-duplicate-banner", new string[] {"RGAA 9.2.1" }},
            {"landmark-no-duplicate-contentinfo", new string[] {"RGAA 9.2.1" }},
            {"landmark-no-duplicate-main", new string[] {"RGAA 9.2.1" }},
            {"landmark-one-main", new string[] {"RGAA 9.2.1" }},
            {"landmark-unique", new string[] {"RGAA 9.2.1" }},
            {"meta-viewport-large", new string[] {"RGAA 13.9.1"}},
            {"region", new string[] {"RGAA 9.2.1" }},
            {"scope-attr-valid", new string[] {"RGAA 5.7.2"}},
            {"tabindex", new string[] {"RGAA 12.8.1"}},
            {"table-duplicate-name", new string[] {"RGAA 15.2.1"}},
            {"meta-refresh-no-exceptions", new string[] {"RGAA 13.1.2"}},
            {"css-orientation-lock", new string[] {"RGAA 13.9.1"}},
            {"hidden-content", new string[] {"RGAA 10.8.1"}},
            {"label-content-name-mismatch", new string[] { "RGAA 11.2.3", "RGAA 11.2.4"}},
            {"table-fake-caption", new string[] {"RGAA 5.1.1"}},
        };

        public override IEnumerable<string> GetTagsByRule(AxeResultItem rule)
        {
            var ruleId = rule.Id;
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
