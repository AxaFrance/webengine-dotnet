﻿using System.Collections.Generic;

namespace AxaFrance.AxeExtended.HtmlReport
{

    /// <summary>
    /// Base class for results: AxePageResult, AxeOverallResult
    /// </summary>
    public abstract class BaseResult
    {

        /// <summary>
        /// Gets the score from test result.
        /// </summary>
        /// <returns>0 to 100</returns>
        protected abstract int GetScore();

        /// <summary>
        /// The sum of score base, that is the sum of all applicable rules.
        /// </summary>
        public int Scorebase { get; set; }

        private int? _score;

        /// <summary>
        /// The score of the test result.
        /// </summary>
        public int? Score
        {
            get
            {
                if (_score == null) GetScore(); return _score;
            }
            set
            {
                _score = value;
            }
        }

        private int? _scoreRotation;


        /// <summary>
        /// The rotation in degree used to display the score in html report
        /// </summary>
        public int? ScoreRotation
        {
            get
            {
                if (_scoreRotation == null) GetScore();
                return _scoreRotation;
            }
            set
            {
                _scoreRotation = value;
            }
        }

        /// <summary>
        /// The color code for the score foreground color.
        /// We want to have a green color for high score and red for low score.
        /// </summary>
        public string ScoreForegroundColor
        {
            get
            {
                var color = scoreToForegroundColor[0];
                foreach (var score in scoreToForegroundColor)
                {
                    if (_score > score.Key)
                    {
                        color = score;
                    }
                    else
                    {
                        break;
                    }

                }
                return color.Value;
            }
        }

        /// <summary>
        /// The color code for the score background color.
        /// </summary>
        public string ScoreBackgroundColor
        {
            get
            {
                var color = scoreToBackgroundColor[0];
                foreach (var score in scoreToBackgroundColor)
                {
                    if (_score > score.Key)
                    {
                        color = score;
                    }
                    else
                    {
                        break;
                    }

                }
                return color.Value;
            }
        }

        private static KeyValuePair<int, string>[] scoreToForegroundColor = new KeyValuePair<int, string>[]
        {
            new KeyValuePair<int, string> (0, "490000"),
            new KeyValuePair<int, string> (50, "970000"),
            new KeyValuePair<int, string> (60, "ff8c00"),
            new KeyValuePair < int, string >(70, "e09d00"),
            new KeyValuePair < int, string >(80, "bab200"),
            new KeyValuePair < int, string >(90, "33dd33"),
        };

        private static KeyValuePair<int, string>[] scoreToBackgroundColor = new KeyValuePair<int, string>[]
{
            new KeyValuePair < int, string >(0, "970000"),
            new KeyValuePair < int, string >(50, "955200"),
            new KeyValuePair < int, string >(60, "745200"),
            new KeyValuePair < int, string >(70, "696400"),
            new KeyValuePair < int, string >(80, "406d0d"),
            new KeyValuePair < int, string >(90, "0d430d"),
};
    }
}
