using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace DotNetOpen.Common
{
    /// <summary>
    /// Is Used to convert an value to required type.
    /// </summary>
    public static class FormatHelper
    {
        public static string FormatHtmlToText(string html, int? maxTextLength = null, bool needReplaceSpecialTag = true)
        {
            string str = string.Empty;
            if (!string.IsNullOrEmpty(html))
            {
                str = Regex.Replace(html, "<[^><]*?>", " ");
                if (needReplaceSpecialTag)
                {
                    str = Regex.Replace(str, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
                    str = Regex.Replace(str, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
                    str = Regex.Replace(str, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
                    str = Regex.Replace(str, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
                    str = Regex.Replace(str, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
                    str = Regex.Replace(str, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
                    str = Regex.Replace(str, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
                    str = Regex.Replace(str, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
                    str = Regex.Replace(str, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
                    str = Regex.Replace(str, @"&#(\d+);", "", RegexOptions.IgnoreCase);
                }
                if (maxTextLength.HasValue && str.Length > maxTextLength.Value && maxTextLength.Value > 3)
                {
                    str = str.Substring(0, maxTextLength.Value - 3) + "...";
                }
            }
            return str;
        }

        public static int CountWords(string html, int? maxTextLength = null, bool needReplaceSpecialTag = true, bool removeSpace = false)
        {
            var formattedStr = FormatHtmlToText(html, maxTextLength, needReplaceSpecialTag);
            formattedStr = formattedStr.Trim();
            if (removeSpace)
            {
                RegexOptions options = RegexOptions.None;
                Regex regex = new Regex("[ ]{2,}", options);
                var isMatch = regex.IsMatch(formattedStr);
                while (isMatch)
                {
                    formattedStr = regex.Replace(formattedStr, " ");
                    isMatch = regex.IsMatch(formattedStr);
                }
            }
            return formattedStr.Split(' ').Length;
        }

        public static string Format(this DateTime dateTime,string shortDateFormatString, string shortTimeFormatStringWithoutSecond, string amDesignator, string pmDesignator)
        {
            var defaultDateTimeFormatInfo = new DateTimeFormatInfo();
            defaultDateTimeFormatInfo.AMDesignator = amDesignator;
            defaultDateTimeFormatInfo.PMDesignator = pmDesignator;
            return dateTime.ToString(string.Format("{0} {1}", shortDateFormatString, shortTimeFormatStringWithoutSecond), defaultDateTimeFormatInfo);
        }
    }
}