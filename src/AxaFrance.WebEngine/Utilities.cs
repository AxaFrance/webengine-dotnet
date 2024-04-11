// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;


namespace AxaFrance.WebEngine
{
    /// <summary>
    /// Common utility class contains shared methods to enhance the generic works
    /// </summary>
    public static class Utilities
    {
        private static Random rand { get; set; } = new Random();

        /// <summary>
        /// Returns a new date calculated from a given date and a duration.
        /// <para>The duration is positive or negative with a unity.</para>
        /// <para>The unity is (D or J), M or (Y or A) representing respectively : a duration expressed in days, month or year.</para>
        /// <para>The sign indicates that the calculated date must be in the past (negative) or in the future (positive).</para>
        /// <para>The start date is optional. If omitted, the current date is considered.</para>
        /// <para>If passed as argument, the start date must be either a string with the following format "DD/MM/YYYY" or a DateTime or even a "differential" date like the duration itself.</para>
        /// <para>In this last case, the startDate is recalculated from the current date.</para> 
        /// <para>Example:</para>
        /// <para>MoveDateByDuration("7D"): returns Today+7 days.</para>
        /// <para>MoveDateByDuration("-6M", DateTime(2017, 6, 25)): should return 25th of december 2016</para>
        /// <para>MoveDateByDuration("-6M", "25/06/2017"): should return 25th of december 2016.</para>
        /// <para>MoveDateByDuration("2D", "1D"): the day in 3 days (2 days from tomorrow).</para>
        /// </summary>
        /// <returns>The calcuated Date of type <see cref="DateTime"/></returns>
        public static DateTime ShiftDateByDuration<T>(String duration, T startDate)
        {
            // Extract the duration as a number
            int number = int.Parse(duration.Substring(0, duration.Length - 1));
            string unity = duration.Substring(duration.Length - 1).ToUpper();

            // init the start date as a DateTime
            DateTime start;

            // If the date passed is a string or is null
            if (startDate.GetType() == typeof(String))
            {
                if (startDate.ToString() == "")
                {
                    start = DateTime.Now;
                }
                else
                {
                    start = ExtractDateFromText(startDate.ToString());
                }
            }
            else
            {
                start = (DateTime)(object)startDate;
            }

            DateTime calculatedDate;

            if (unity == "D" || unity == "J")
            {
                calculatedDate = start.AddDays(number);
            }
            else if (unity == "M")
            {
                calculatedDate = start.AddMonths(number);
            }
            else if (unity == "Y" || unity == "A")
            {
                calculatedDate = start.AddYears(number);
            }
            else
            {
                throw new WebEngineGeneralException(string.Format("The startDate value {0} is not valid.", startDate));
            }

            return calculatedDate;
        }


        /// <summary>
        /// Generate a random number between 0 and the value.
        /// </summary>
        /// <param name="val">The max value.</param>
        /// <returns>Integer</returns>
        public static int GetRandomNumber(int val)
        {
            return rand.Next(val);
        }


        /// <summary>
        /// Return a random letter (uppercase).
        /// </summary>
        /// <returns>String</returns>
        public static String GetRandomLetter()
        {
            String alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int position = GetRandomNumber(26);

            return alphabet.Substring(position, 1);
        }


        /// <summary>
        /// Return an email based on first and last name.
        /// No transformation is done on last name and first names.
        /// The email is considered unique as the date is injected in the construction.
        /// </summary>
        /// <param name="lastname">The last name.</param>
        /// <param name="firstname">The first name.</param>
        /// <param name="isUnique">Indicates if the generated email must be unique or not. Default: true</param>
        /// <param name="domain">The domain of the generated mail. Default: gmail.com</param>
        /// <returns>An email.</returns>
        public static string GenerateEmail(string lastname, string firstname, bool isUnique = true, string domain = "test.com")
        {
            String stringPart = firstname.Replace(' ', '_').ToLower() + '.' + lastname.Replace(' ', '_').ToLower();
            String uniquePart = "";

            if (isUnique)
            {
                DateTime date = new DateTime();
                uniquePart = date.Year.ToString() + date.Month.ToString() + date.Day.ToString();
            }

            return stringPart + uniquePart + '@' + domain;
        }


        /// <summary>
        /// Return a phone number composed by a fixed base and the 4 last digits randomly generated.
        /// </summary>
        /// <param name="numberBase">The first part of the phone number. Default: 010000 (french format)</param>
        /// <returns>A phone number</returns>
        public static String GeneratePhoneNumber(String numberBase = "01000")
        {
            return numberBase + GetRandomNumber(9999).ToString("0000");
        }


        /// <summary>
        /// Turn the string "JJ/MM/AAAA" into a date
        /// </summary>
        /// <param name="text">The string to convert</param>
        /// <returns>The date represented by the String. If no date is contained in the string or wrong format, the current date is returned</returns>
        public static DateTime ExtractDateFromText(string text)
        {
            string pattern = @"(\d{2}/\d{2}/\d{4})";
            Match match = Regex.Match(text, pattern);
            if (match.Success)
            {
                string dateString = match.Value;
                return DateTime.Parse(dateString, new CultureInfo("fr-FR").DateTimeFormat);
            }
            else
            {
                return new DateTime();
            }
        }


        /// <summary>
        /// Saves the content into a file
        /// </summary>
        /// <param name="filename">The file name to be saved.</param>
        /// <param name="content">The content to save.</param>
        public static void SaveFile(string filename, string content)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.Write(content);
            }
        }


        /// <summary>
        /// Generates a Lorem Ipsum text.
        /// </summary>
        /// <param name="minWords">The min number of words in the generated text.</param>
        /// <param name="maxWords">The max number of words in the generated text.</param>
        /// <param name="minSentences">The min number of sentences in the generated text.</param>
        /// <param name="maxSentences">The max number of sentences in the generated text.</param>
        /// <param name="numParagraphs">The number of paragraphs in the generated text.</param>
        /// <returns>A Lorem Ipsum string of the defined size.</returns>
        public static string LoremIpsum(int minWords,
                                           int maxWords,
                                        int minSentences,
                                        int maxSentences,
                                        int numParagraphs)
        {

            var words = new[]{"lorem", "ipsum", "dolor", "sit", "amet", "consectetuer",
                "adipiscing", "elit", "sed", "diam", "nonummy", "nibh", "euismod",
                "tincidunt", "ut", "laoreet", "dolore", "magna", "aliquam", "erat"};

            var rand = new Random();
            int numSentences = rand.Next(maxSentences - minSentences)
                + minSentences + 1;
            int numWords = rand.Next(maxWords - minWords) + minWords + 1;

            StringBuilder result = new StringBuilder();

            for (int p = 0; p < numParagraphs; p++)
            {
                for (int s = 0; s < numSentences; s++)
                {
                    for (int w = 0; w < numWords; w++)
                    {
                        if (w > 0) { result.Append(" "); }
                        result.Append(words[rand.Next(words.Length)]);
                    }
                    result.Append(". ");
                }
                result.Append("\n");
            }

            return result.ToString();
        }


        /// <summary>
        /// Randomly select an item from a list.
        /// If the field has single or double quotes, quotes are removed.
        /// </summary>
        /// <param name="items">List of items</param>
        /// <returns>Picked-up item</returns>
        public static object GetRandomStringFromList(object[] items)
        {
            int index = rand.Next(items.Length);
            object chosenItem = items[index];
            if (chosenItem is string)
            {
                string s = chosenItem as string;
                s = s.Trim('\'', '"', '\t', ' ', '\r');
                return s;
            }
            return chosenItem;
        }


        /// <summary>
        /// Picks up a random value from an environment variable containing a list of values.
        /// </summary>
        /// <param name="variable">The name of the environment variable</param>
        /// <param name="separator">The list separator used to define the environment variable. Default: ';'</param>
        /// <returns>String</returns>
        public static string GetRandomItemFromEnvironmentVariable(string variable, char separator = ';')
        {
            string list = EnvironmentVariables.Current.GetValue(variable);
            string[] names = list.Split(separator);
            return AxaFrance.WebEngine.Utilities.GetRandomStringFromList(names).ToString();
        }

        internal static string GetDescriptionByType(Type t)
        {
            var attr = t.GetCustomAttribute<DescriptionAttribute>();
            if (attr != null)
            {
                return attr.Description;
            }
            else
            {
                return t.Name;
            }
        }

        /// <summary>
        /// Finds the description of an action from className.
        /// </summary>
        /// <param name="className">class name</param>
        /// <returns>The description of the class </returns>
        internal static string GetDescriptionByClassName(string className)
        {
            try
            {
                Type t = GetTypeByClassName(className);
                return GetDescriptionByType(t);
            }
            catch
            {
                return className;
            }
        }

        internal static Type GetTypeByClassName(string className, object classInAssembly)
        {
            Assembly ass = classInAssembly.GetType().Assembly;
            Type[] types = ass.GetTypes();
            Type t = types.FirstOrDefault(x => x.Name == className);
            if (t != null)
            {
                return t;
            }
            else
            {
                return GetTypeByClassName(className);
            }
        }

        internal static Type GetTypeByClassName(string className)
        {
            try
            {
                foreach (Assembly ass in GlobalConstants.LoadedAssemblies)
                {
                    Type[] types = ass.GetTypes();
                    Type t = types.FirstOrDefault(x => x.Name == className);
                    if (t != null)
                    {
                        return t;
                    }
                }
            }
            catch
            {

            }
            throw new ActionNotFoundException($"The action type {className} can not be found in loaded assemblies.");
        }
    }
}
