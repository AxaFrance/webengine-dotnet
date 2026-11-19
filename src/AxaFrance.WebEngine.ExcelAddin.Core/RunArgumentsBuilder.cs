// Copyright (c) 2016-2026 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Text;
using AxaFrance.WebEngine;

namespace AxaFrance.WebEngine.ExcelAddin
{
    /// <summary>
    /// Executable and arguments used to launch WebRunner.
    /// </summary>
    public sealed record RunnerCommand(string FileName, string Arguments, string WorkingDirectory);

    /// <summary>
    /// Builds the WebRunner command line from the add-in settings.
    /// </summary>
    public static class RunArgumentsBuilder
    {
        /// <summary>
        /// Builds the WebRunner argument string.
        /// </summary>
        public static string BuildArguments(
            string testAssembly,
            string testDataFile,
            string environmentFile,
            BrowserType browser,
            string? platform = null,
            string? device = null,
            string? appId = null,
            bool manualMode = false,
            bool showReport = false)
        {
            var builder = new StringBuilder();
            AppendValue(builder, "-a:", testAssembly);
            AppendValue(builder, "-data:", testDataFile);
            AppendValue(builder, "-env:", environmentFile);
            AppendValue(builder, "-browser:", browser.ToString());
            builder.Append(" -u");


            if (!string.IsNullOrWhiteSpace(platform))
            {
                AppendValue(builder, "-platform:", platform);
            }

            if (!string.IsNullOrWhiteSpace(device))
            {
                AppendValue(builder, "-device:", device);
            }

            if (!string.IsNullOrWhiteSpace(appId))
            {
                AppendValue(builder, "-appid:", appId);
            }

            if (manualMode)
            {
                builder.Append(" -m");
            }

            if (showReport)
            {
                builder.Append(" -showreport");
            }

            return builder.ToString().Trim();
        }

        /// <summary>
        /// Resolves WebRunner.exe, or WebRunner.jar with java.exe.
        /// </summary>
        public static RunnerCommand ResolveRunner(string webRunnerPath, string arguments)
        {
            if (string.IsNullOrWhiteSpace(webRunnerPath))
            {
                throw new InvalidOperationException("WebRunner folder is not configured. Please open Settings first.");
            }

            var directory = new DirectoryInfo(webRunnerPath);
            if (!directory.Exists)
            {
                throw new DirectoryNotFoundException($"WebRunner folder not found: {webRunnerPath}");
            }

            string executable = Path.Combine(directory.FullName, "WebRunner.exe");
            if (File.Exists(executable))
            {
                return new RunnerCommand(executable, arguments, directory.FullName);
            }

            string jar = Path.Combine(directory.FullName, "WebRunner.jar");
            if (File.Exists(jar))
            {
                string bundledJava = Path.Combine(directory.FullName, "jre", "bin", "java.exe");
                string java = File.Exists(bundledJava) ? bundledJava : "java.exe";
                return new RunnerCommand(java, $"-jar {Quote(jar)} {arguments}", directory.FullName);
            }

            throw new FileNotFoundException(
                $"WebRunner.exe or WebRunner.jar was not found in '{webRunnerPath}'. Please check the Settings.");
        }

        private static void AppendValue(StringBuilder builder, string prefix, string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            if (builder.Length > 0)
            {
                builder.Append(' ');
            }

            builder.Append(prefix).Append(Quote(value));
        }

        private static string Quote(string value)
        {
            return "\"" + value.Replace("\"", "\\\"") + "\"";
        }
    }
}
