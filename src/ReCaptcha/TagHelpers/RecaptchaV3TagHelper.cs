using System;
using System.Text.Encodings.Web;
using Griesoft.AspNetCore.ReCaptcha.Configuration;
using Griesoft.AspNetCore.ReCaptcha.Localization;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

#if !NET461
using Microsoft.AspNetCore.Mvc.TagHelpers;
#else
using Griesoft.AspNetCore.ReCaptcha.Extensions;
#endif

namespace Griesoft.AspNetCore.ReCaptcha.TagHelpers
{
    /// <summary>
    /// A reCAPTCHA V3 tag helper, which can be used to automatically bind the challenge to a button.
    /// </summary>
    /// <remarks>
    /// The <see cref="FormId"/> is required. With the exception that you set a <see cref="Callback"/> instead.
    /// When setting both the value set to <c>Callback</c> always wins and the <c>FormId</c> value is basically irrelevant.
    /// 
    /// For easiest use of this tag helper set only the <c>FormId</c>. This will add a default callback function to the body. That function does
    /// submit the form after a successful reCAPTCHA challenge.
    /// 
    /// If the tag is not inside the form that is going to be submitted, you should use a custom callback function. The default callback function
    /// does not add the reCAPTCHA token to the form, which will result in response verification failure.
    /// </remarks>
    /// <example>
    /// The simplest use of the tag would be:
    /// <code>
    /// <recaptchav3 formid="myForm" action="submit">Submit</recaptchav3>
    /// </code>
    /// 
    /// Which will translate into the following HTML:
    /// <code>
    /// <button class="g-recaptcha" data-sitekey="your_site_key" data-callback='submitmyForm' data-action='submit'>Submit</button>
    /// </code>
    /// </example>
    [HtmlTargetElement("recaptcha-v3", Attributes = "callback,action")]
    [HtmlTargetElement("recaptcha-v3", Attributes = "form-id,action")]
    public class RecaptchaV3TagHelper : TagHelper
    {
        private readonly ITagHelperComponentManager _tagHelperComponentManager;
        private readonly RecaptchaSettings _settings;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="tagHelperComponentManager"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public RecaptchaV3TagHelper(IOptionsMonitor<RecaptchaSettings> settings, ITagHelperComponentManager tagHelperComponentManager)
        {
            _ = settings ?? throw new ArgumentNullException(nameof(settings));
            _ = tagHelperComponentManager ?? throw new ArgumentNullException(nameof(tagHelperComponentManager));

            _settings = settings.CurrentValue;
            _tagHelperComponentManager = tagHelperComponentManager;
        }

        /// <summary>
        /// The id of the form that will be submitted after a successful reCAPTCHA challenge. 
        /// </summary>
        /// <remarks>This does only apply when not specifying a <see cref="Callback"/>.</remarks>
        public string? FormId { get; set; }

        /// <summary>
        /// Set the name of your callback function, which is called when the reCAPTCHA challenge was successful. 
        /// A "g-recaptcha-response" token is added to your callback function parameters for server-side verification.
        /// </summary>
        public string? Callback { get; set; }

        /// <summary>
        /// The name of the action that was triggered.
        /// </summary>
        /// <remarks>You should verify that the server-side verification response returns the same action.</remarks>
        public string Action { get; set; } = string.Empty;

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">Thrown when both <see cref="Callback"/> and <see cref="FormId"/> or <see cref="Action"/> are/is null or empty.</exception>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            _ = output ?? throw new ArgumentNullException(nameof(output));

            if (string.IsNullOrEmpty(Callback) && string.IsNullOrEmpty(FormId))
            {
                throw new InvalidOperationException(Resources.CallbackPropertyNullErrorMessage);
            }

            if (string.IsNullOrEmpty(Action))
            {
                throw new InvalidOperationException(Resources.ActionPropertyNullErrorMessage);
            }

            if (string.IsNullOrEmpty(Callback))
            {
                Callback = $"submit{FormId}";
                _tagHelperComponentManager.Components.Add(new CallbackScriptTagHelperComponent(FormId!));
            }

            output.TagMode = TagMode.StartTagAndEndTag;

            output.TagName = "button";
            output.AddClass("g-recaptcha", HtmlEncoder.Default);

            output.Attributes.SetAttribute("data-sitekey", _settings.SiteKey);
            output.Attributes.SetAttribute("data-callback", Callback);
            output.Attributes.SetAttribute("data-action", Action);
        }
    }
}
