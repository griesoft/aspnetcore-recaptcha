using System;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Griesoft.AspNetCore.ReCaptcha.TagHelpers
{
    /// <summary>
    /// This tag helper component is used to add a short callback script to the bottom of a body tag.
    /// </summary>
    /// <remarks>
    /// The callback script is used as a default callback function to submit a form after a reCAPTCHA challenge was successful.
    /// </remarks>
    public class CallbackScriptTagHelperComponent : TagHelperComponent
    {
        private readonly string _formId;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formId"></param>
        public CallbackScriptTagHelperComponent(string formId)
        {
            _formId = formId;
        }

        /// <inheritdoc />
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (string.Equals(context.TagName, "body", StringComparison.OrdinalIgnoreCase))
            {
                output.PostContent.AppendHtml(CallbackScript(_formId));
            }
        }

        private static string CallbackScript(string formId)
        {
            // Append the formId to the function name in case that multiple recaptcha tags are added in a document.
            return $"<script>function submit{formId}(token){{document.getElementById('{formId}').submit();}}</script>";
        }
    }
}
