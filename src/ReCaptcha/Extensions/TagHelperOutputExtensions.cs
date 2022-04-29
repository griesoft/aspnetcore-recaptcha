using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Griesoft.AspNetCore.ReCaptcha.Extensions
{
    // Copied some extension methods from https://github.com/aspnet/Mvc/blob/release/2.2/src/Microsoft.AspNetCore.Mvc.TagHelpers/TagHelperOutputExtensions.cs,
    // because they were only available for .NET Core 2.2+. 
    // Also borrowing code from https://github.com/aspnet/HttpAbstractions/blob/master/src/Microsoft.AspNetCore.WebUtilities/QueryHelpers.cs#L63
    // to reduce dependency count, because we only need this one functionality from the package.
    internal static class TagHelperOutputExtensions
    {
        private static readonly char[] SpaceChars = { '\u0020', '\u0009', '\u000A', '\u000C', '\u000D' };

        internal static string AddQueryString(string uri, IEnumerable<KeyValuePair<string, string>> queryString)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            if (queryString == null)
            {
                throw new ArgumentNullException(nameof(queryString));
            }

            var anchorIndex = uri.IndexOf('#');
            var uriToBeAppended = uri;
            var anchorText = "";
            // If there is an anchor, then the query string must be inserted before its first occurence.
            if (anchorIndex != -1)
            {
                anchorText = uri.Substring(anchorIndex);
                uriToBeAppended = uri.Substring(0, anchorIndex);
            }

            var queryIndex = uriToBeAppended.IndexOf('?');
            var hasQuery = queryIndex != -1;

            var sb = new StringBuilder();
            sb.Append(uriToBeAppended);
            foreach (var parameter in queryString)
            {
                sb.Append(hasQuery ? '&' : '?');
                sb.Append(UrlEncoder.Default.Encode(parameter.Key));
                sb.Append('=');
                sb.Append(UrlEncoder.Default.Encode(parameter.Value));
                hasQuery = true;
            }

            sb.Append(anchorText);
            return sb.ToString();
        }

        internal static void AddClass(this TagHelperOutput tagHelperOutput, string classValue, HtmlEncoder htmlEncoder)
        {
            if (tagHelperOutput == null)
            {
                throw new ArgumentNullException(nameof(tagHelperOutput));
            }

            if (string.IsNullOrEmpty(classValue))
            {
                return;
            }

            var encodedSpaceChars = SpaceChars.Where(x => !x.Equals('\u0020')).Select(x => htmlEncoder.Encode(x.ToString(CultureInfo.InvariantCulture))).ToArray();

            if (SpaceChars.Any(classValue.Contains) || encodedSpaceChars.Any(value => classValue.Contains(value)))
            {
                throw new ArgumentException(null, nameof(classValue));
            }

            if (!tagHelperOutput.Attributes.TryGetAttribute("class", out var classAttribute))
            {
                tagHelperOutput.Attributes.Add("class", classValue);
            }
            else
            {
                var currentClassValue = ExtractClassValue(classAttribute, htmlEncoder);

                var encodedClassValue = htmlEncoder.Encode(classValue);

                if (string.Equals(currentClassValue, encodedClassValue, StringComparison.Ordinal))
                {
                    return;
                }

                var arrayOfClasses = currentClassValue.Split(SpaceChars, StringSplitOptions.RemoveEmptyEntries)
                    .SelectMany(perhapsEncoded => perhapsEncoded.Split(encodedSpaceChars, StringSplitOptions.RemoveEmptyEntries))
                    .ToArray();

                if (arrayOfClasses.Contains(encodedClassValue, StringComparer.Ordinal))
                {
                    return;
                }

                var newClassAttribute = new TagHelperAttribute(
                    classAttribute.Name,
                    new HtmlString($"{currentClassValue} {encodedClassValue}"),
                    classAttribute.ValueStyle);

                tagHelperOutput.Attributes.SetAttribute(newClassAttribute);
            }
        }

        private static string ExtractClassValue(TagHelperAttribute classAttribute, HtmlEncoder htmlEncoder)
        {
            string? extractedClassValue;
            switch (classAttribute.Value)
            {
                case string valueAsString:
                    extractedClassValue = htmlEncoder.Encode(valueAsString);
                    break;
                case HtmlString valueAsHtmlString:
                    extractedClassValue = valueAsHtmlString.Value;
                    break;
                case IHtmlContent htmlContent:
                    using (var stringWriter = new StringWriter())
                    {
                        htmlContent.WriteTo(stringWriter, htmlEncoder);
                        extractedClassValue = stringWriter.ToString();
                    }
                    break;
                default:
                    extractedClassValue = htmlEncoder.Encode(classAttribute.Value.ToString() ?? string.Empty);
                    break;
            }
            var currentClassValue = extractedClassValue ?? string.Empty;
            return currentClassValue;
        }
    }
}
