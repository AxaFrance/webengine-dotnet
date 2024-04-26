using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axe.Extended.HtmlReport
{
    /// <summary>
    /// Outpur format of the HTML report
    /// </summary>
    public enum OutputFormat
    {
        /// <summary>
        /// HTML report and all resources compressed into a single file. Ideal when accessibility testing is part of a CI/CD pipeline.
        /// </summary>
        Zip,

        /// <summary>
        /// HTML report and all resources in a folder. Ideal when accessibility testing is running locally.
        /// </summary>
        Html
    }
}
