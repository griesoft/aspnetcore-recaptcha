using System;
using System.Text.Encodings.Web;
using Griesoft.AspNetCore.ReCaptcha.Configuration;
using Griesoft.AspNetCore.ReCaptcha.Extensions;
using Griesoft.AspNetCore.ReCaptcha.Localization;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

namespace Griesoft.AspNetCore.ReCaptcha.TagHelpers
{
    /// <summary>
    /// Add a invisible reCAPTCHA div element to your page or automatically bind the invisible captcha to a button element
    /// by adding a re-invisible attribute to it. Both require in addition the callback attribute.
    /// </summary>
    [HtmlTargetElement("recaptcha-invisible", Attributes = "callback", TagStructure = TagStructure.WithoutEndTag)]
    [HtmlTargetElement("button", Attributes = "re-invisible,callback")]
    public class RecaptchaInvisibleTagHelper : TagHelper
    {
        private readonly RecaptchaSettings _settings;
        private readonly RecaptchaOptions _options;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="options"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public RecaptchaInvisibleTagHelper(IOptionsMonitor<RecaptchaSettings> settings, IOptionsMonitor<RecaptchaOptions> options)
        {
            _ = settings ?? throw new ArgumentNullException(nameof(settings));
            _ = options ?? throw new ArgumentNullException(nameof(options));

            _settings = settings.CurrentValue;
            _options = options.CurrentValue;

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
        /// Set the name of your callback function, executed when the user submits a successful response. The "g-recaptcha-response" token is passed to your callback.
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
        /// <exception cref="NullReferenceException"><see cref="Callback"/> must not be null. It is required for invisible reCAPTCHA to work.</exception>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            _ = output ?? throw new ArgumentNullException(nameof(output));

            if (string.IsNullOrEmpty(Callback))
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
