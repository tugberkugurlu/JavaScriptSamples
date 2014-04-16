using System;
using System.Text;
using System.Text.RegularExpressions;

namespace TinyMCESample
{
    public static class StringExtensions
    {
        /// <summary>
        /// A regular expression for validating slugs.
        /// Does not allow leading or trailing hypens or whitespace
        /// </summary>
        private static readonly Regex SlugRegex = new Regex(@"(^[a-z0-9])([a-z0-9_-]+)*([a-z0-9])$", RegexOptions.Compiled);

        /// <summary>
        /// Slugifies a string
        /// </summary>
        /// <param name="value">The string value to slugify</param>
        /// <param name="maxLength">An optional maximum length of the generated slug</param>
        /// <returns>A URL safe slug representation of the input <paramref name="value"/>.</returns>
        public static string ToSlug(this string value, int? maxLength = null)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            // if it's already a valid slug, return it
            if (SlugRegex.IsMatch(value))
            {
                return value;
            }

            return GenerateSlug(value, maxLength);
        }

        /// <summary>
        /// Credit for this method goes to http://stackoverflow.com/questions/2920744/url-slugify-alrogithm-in-cs
        /// and https://github.com/benfoster/Fabrik.Common/blob/master/src/Fabrik.Common/StringExtensions.cs
        /// </summary>
        private static string GenerateSlug(string value, int? maxLength = null)
        {
            // prepare string, remove accents, lower case and convert hyphens to whitespace
            var result = RemoveAccent(value).Replace("-", " ").ToLowerInvariant();

            result = Regex.Replace(result, @"[^a-z0-9\s-]", string.Empty); // remove invalid characters
            result = Regex.Replace(result, @"\s+", " ").Trim(); // convert multiple spaces into one space

            if (maxLength.HasValue) // cut and trim
            {
                result = result.Substring(0, result.Length <= maxLength ? result.Length : maxLength.Value).Trim();
            }

            return Regex.Replace(result, @"\s", "-"); // replace all spaces with hyphens
        }

        private static string RemoveAccent(string value)
        {
            var bytes = Encoding.GetEncoding("Cyrillic").GetBytes(value);
            return Encoding.ASCII.GetString(bytes);
        }
    }
}