using System;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Griesoft.AspNetCore.ReCaptcha.Extensions
{
    // Copied some extension methods from https://github.com/aspnet/Mvc/blob/release/2.2/src/Microsoft.AspNetCore.Mvc.TagHelpers/TagHelperOutputExtensions.cs,
    // because they were only available for .NET Core 2.2+
    internal static class TagHelperOutputExtensions
    {
        private static readonly char[] SpaceChars = { '\u0020', '\u0009', '\u000A', '\u000C', '\u000D' };

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

            var encodedSpaceChars = SpaceChars.Where(x => !x.Equals('\u0020')).Select(x => htmlEncoder.Encode(x.ToString())).ToArray();

            if (SpaceChars.Any(classValue.Contains) || encodedSpaceChars.Any(value => classValue.IndexOf(value, StringComparison.Ordinal) >= 0))
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
            string extractedClassValue;
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
                    extractedClassValue = htmlEncoder.Encode(classAttribute.Value?.ToString());
                    break;
            }
            var currentClassValue = extractedClassValue ?? string.Empty;
            return currentClassValue;
        }
    }
}
