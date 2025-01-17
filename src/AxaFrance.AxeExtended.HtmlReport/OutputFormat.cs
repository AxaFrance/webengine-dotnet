namespace AxaFrance.AxeExtended.HtmlReport
{
    /// <summary>
    /// Output format of the test report.
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
