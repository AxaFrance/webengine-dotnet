// Copyright (c) 2016-2026 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Text;

namespace AxaFrance.WebEngine.ExcelAddin
{
    /// <summary>
    /// Generates parameter constant classes from the PARAMS worksheet.
    /// </summary>
    public static class CodeGenerator
    {
        /// <summary>
        /// Generates a C# parameter class.
        /// </summary>
        public static string GenerateCSharp(IEnumerable<ParameterDefinition> parameters)
        {
            ArgumentNullException.ThrowIfNull(parameters);

            var builder = new StringBuilder();
            builder.AppendLine("public static class ParameterList {");
            foreach (var parameter in parameters)
            {
                AppendParameter(builder, parameter, "\t///<Summary>Parameter: {0} </Summary>", "\n\t/// ", "\tpublic static string {0} {{get; }} = \"{1}\";");
            }
            builder.AppendLine("}");
            return builder.ToString();
        }

        /// <summary>
        /// Generates a Java parameter class.
        /// </summary>
        public static string GenerateJava(IEnumerable<ParameterDefinition> parameters)
        {
            ArgumentNullException.ThrowIfNull(parameters);

            var builder = new StringBuilder();
            builder.AppendLine("public class ParameterList {");
            foreach (var parameter in parameters)
            {
                AppendParameter(builder, parameter, "\t/** Parameter: {0} */", "\n\t * ", "\tpublic static final String {0} = \"{1}\";");
            }
            builder.AppendLine("}");
            return builder.ToString();
        }

        private static void AppendParameter(
            StringBuilder builder,
            ParameterDefinition parameter,
            string commentTemplate,
            string multiLineSeparator,
            string propertyTemplate)
        {
            string comment = string.IsNullOrWhiteSpace(parameter.Description)
                ? string.Format(commentTemplate, parameter.Name)
                : string.Format(commentTemplate, parameter.Description.Replace("\n", multiLineSeparator));

            builder.AppendLine(comment);
            builder.AppendLine(string.Format(propertyTemplate, parameter.Name, parameter.Name));
            builder.AppendLine();
        }
    }
}
