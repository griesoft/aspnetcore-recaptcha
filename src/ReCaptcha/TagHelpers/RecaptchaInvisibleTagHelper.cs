using System;
using System.Text.Encodings.Web;
using Griesoft.AspNetCore.ReCaptcha.Configuration;
using Griesoft.AspNetCore.ReCaptcha.Extensions;
using Griesoft.AspNetCore.ReCaptcha.Localization;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

namespace Griesoft.AspNetCore.ReCaptcha.TagHelpers
{
    /// <summary>
    /// Add a invisible reCAPTCHA div element to your page. You may also use it by adding a 're-invisible' attribute to a button 
    /// element which will automatically bind the challenge to it.
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
    /// <button re-invisible formid="myForm">Submit</button>
    /// </code>
    /// 
    /// Which will translate into the following HTML:
    /// <code>
    /// <button class="g-recaptcha" data-sitekey="your_site_key" data-callback="submitmyForm" data-badge="bottomleft">Submit</button>
    /// </code>
    /// </example>
    [HtmlTargetElement("recaptcha-invisible", Attributes = "callback", TagStructure = TagStructure.WithoutEndTag)]
    [HtmlTargetElement("recaptcha-invisible", Attributes = "form-id", TagStructure = TagStructure.WithoutEndTag)]
    [HtmlTargetElement("button", Attributes = "re-invisible,callback")]
    [HtmlTargetElement("button", Attributes = "re-invisible,form-id")]
    public class RecaptchaInvisibleTagHelper : TagHelper
    {
        private readonly ITagHelperComponentManager _tagHelperComponentManager;
        private readonly RecaptchaSettings _settings;
        private readonly RecaptchaOptions _options;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="options"></param>
        /// <param name="tagHelperComponentManager"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public RecaptchaInvisibleTagHelper(IOptionsMonitor<RecaptchaSettings> settings, IOptionsMonitor<RecaptchaOptions> options,
            ITagHelperComponentManager tagHelperComponentManager)
        {
            _ = settings ?? throw new ArgumentNullException(nameof(settings));
            _ = options ?? throw new ArgumentNullException(nameof(options));
            _ = tagHelperComponentManager ?? throw new ArgumentNullException(nameof(tagHelperComponentManager));

            _settings = settings.CurrentValue;
            _options = options.CurrentValue;
            _tagHelperComponentManager = tagHelperComponentManager;

            Badge = _options.Badge;
        }

        /// <summary>
        /// Set the badge position for the reCAPTCHA element.
        /// Set this to <see cref="BadgePosition.Inline"/> when you want to position it with CSS yourself.
        /// </summary>
        public BadgePosition Badge { get; set; }

        /// <summary>
        /// Set the tabindex of the reCAPTCHA element. If other elements in your page use tabindex, it should be set to make user navigation easier.
        /// </summary>
        public int? TabIndex { get; set; } = null;

        /// <summary>
        /// The id of the form that will be submitted after a successful reCAPTCHA challenge. 
        /// </summary>
        /// <remarks>This does only apply when not specifying a <see cref="Callback"/>.</remarks>
        public string? FormId { get; set; }

        /// <summary>
        /// Set the name of your callback function, which is called when the reCAPTCHA challenge was successful. 
        /// A "g-recaptcha-response" token is added to your callback function parameters for server-side verification.
        /// </summary>
        public string Callback { get; set; } = string.Empty;

        /// <summary>
        /// Set the name of your callback function, executed when the reCAPTCHA response expires and the user needs to re-verify.
        /// </summary>
        public string? ExpiredCallback { get; set; }

        /// <summary>
        /// Set the name of your callback function, executed when reCAPTCHA encounters an error (usually network connectivity) and cannot continue until connectivity is restored. 
        /// If you specify a function here, you are responsible for informing the user that they should retry.
        /// </summary>
        public string? ErrorCallback { get; set; }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NullReferenceException">Thrown when both <see cref="Callback"/> and <see cref="FormId"/> are null or empty.</exception>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            _ = output ?? throw new ArgumentNullException(nameof(output));

            if (string.IsNullOrEmpty(Callback) && string.IsNullOrEmpty(FormId))
            {
                throw new NullReferenceException(Resources.CallbackPropertyNullErrorMessage);
            }

            if (output.TagName == "button")
            {
                output.Attributes.Remove(output.Attributes["re-invisible"]);
            }
            else
            {
                output.TagName = "div";

                output.Attributes.SetAttribute("data-size", "invisible");
            }

            if (string.IsNullOrEmpty(Callback))
            {
                Callback = $"submit{FormId}";
                _tagHelperComponentManager.Components.Add(new CallbackScriptTagHelperComponent(FormId!));
            }

            output.TagMode = TagMode.StartTagAndEndTag;

            output.AddClass("g-recaptcha", HtmlEncoder.Default);

            output.Attributes.SetAttribute("data-sitekey", _settings.SiteKey);
            output.Attributes.SetAttribute("data-badge", Badge.ToString().ToLowerInvariant());
            output.Attributes.SetAttribute("data-callback", Callback);

            if (TabIndex != null)
            {
                output.Attributes.SetAttribute("data-tabindex", TabIndex);
            }

            if (!string.IsNullOrEmpty(ExpiredCallback))
            {
                output.Attributes.SetAttribute("data-expired-callback", ExpiredCallback);
            }

            if (!string.IsNullOrEmpty(ErrorCallback))
            {
                output.Attributes.SetAttribute("data-error-callback", ErrorCallback);
            }
        }
    }
}
