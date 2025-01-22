using System;
using System.Xml.Serialization;

namespace AxaFrance.WebEngine.Report
{
    /// <summary>
    /// Additional data for the report (it can be in future accessibility report, performance report, etc.)
    /// </summary>
    [Serializable]
    [XmlRoot(Namespace = GlobalConstants.XmlNamespace)]
    public class AdditionalData
    {
        /// <summary>
        /// Name of the additional data: possible value: "AccessibilityReport", ... may be extended in future
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// RAW content of the additional data
        /// </summary>
        public byte[] Value { get; set; }
    }
}
