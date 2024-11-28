using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Star_Wars_D6
{
    public static class HTMLConversion
    {
        /// <summary>
        /// Converts an HTML string to plain text and extracts specific difficulties.
        /// </summary>
        /// <param name="html">The HTML string to process.</param>
        /// <returns>A dictionary with difficulty labels as keys and extracted values as values.</returns>
        public static Dictionary<string, string> ExtractDifficulties(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
            {
                return new Dictionary<string, string> { { "Error", "No description available." } };
            }

            try
            {
                // Convert HTML to plain text
                string plainText = ConvertHtmlToPlainText(html);

                // Define refined difficulty patterns to capture only the required parts
                var difficultyPatterns = new[]
                {
            @"(Control Difficulty):\s*(.*?)(?:\n|$)", // Control Difficulty until newline or end of string
            @"(Sense Difficulty):\s*(.*?)(?:\n|$)",  // Sense Difficulty until newline or end of string
            @"(Alter Difficulty):\s*(.*?)(?:\n|$)"   // Alter Difficulty until newline or end of string
        };

                // Extract difficulties using refined patterns
                var extractedDifficulties = new Dictionary<string, string>();
                foreach (var pattern in difficultyPatterns)
                {
                    var match = Regex.Match(plainText, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    if (match.Success)
                    {
                        string label = match.Groups[1].Value.Trim();
                        string value = match.Groups[2].Value.Trim();

                        // Ensure the value is not part of the Effect or unnecessary text
                        value = CleanUpDifficulty(value);

                        if (!string.IsNullOrEmpty(value))
                        {
                            extractedDifficulties[label] = value;
                        }
                    }
                }

                // Return extracted difficulties or a fallback if none found
                return extractedDifficulties.Any()
                    ? extractedDifficulties
                    : new Dictionary<string, string> { { "Error", "Short description not available." } };
            }
            catch (Exception ex)
            {
                return new Dictionary<string, string> { { "Error", $"Error extracting difficulties: {ex.Message}" } };
            }
        }

        /// <summary>
        /// Converts HTML content to plain text.
        /// </summary>
        /// <param name="html">The HTML string to convert.</param>
        /// <returns>A plain text representation of the HTML.</returns>
        private static string ConvertHtmlToPlainText(string html)
        {
            // Remove HTML tags
            string plainText = Regex.Replace(html, "<.*?>", string.Empty);

            // Replace common HTML entities
            plainText = plainText.Replace("&nbsp;", " ")
                                 .Replace("&amp;", "&")
                                 .Replace("&lt;", "<")
                                 .Replace("&gt;", ">")
                                 .Replace("&quot;", "\"")
                                 .Replace("&#39;", "'");

            return plainText.Trim();
        }

        /// <summary>
        /// Cleans up the extracted difficulty text to remove unnecessary or trailing information.
        /// </summary>
        /// <param name="difficulty">The raw difficulty text.</param>
        /// <returns>The cleaned-up difficulty text.</returns>
        private static string CleanUpDifficulty(string difficulty)
        {
            // Remove "Effect" or similar trailing content if it exists
            int effectIndex = difficulty.IndexOf("Effect", StringComparison.OrdinalIgnoreCase);
            if (effectIndex >= 0)
            {
                difficulty = difficulty.Substring(0, effectIndex).Trim();
            }

            // Further clean up any leftover artifacts
            return Regex.Replace(difficulty, @"\s{2,}", " ").Trim(); // Replace multiple spaces with a single space
        }
    }
    }
