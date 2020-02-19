using System;
using System.Text.Encodings.Web;
using GSoftware.AspNetCore.ReCaptcha.Configuration;
using GSoftware.AspNetCore.ReCaptcha.Extensions;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

namespace GSoftware.AspNetCore.ReCaptcha.TagHelpers
{
    /// <summary>
    /// A tag helper which adds a Google reCAPTCHA div element to your page.
    /// </summary>
    public class RecaptchaTagHelper : TagHelper
    {
        private readonly RecaptchaSettings _settings;

        public RecaptchaTagHelper(IOptionsMonitor<RecaptchaSettings> settings)
        {
            _ = settings ?? throw new ArgumentNullException(nameof(settings));

            _settings = settings.CurrentValue;
        }

        /// <summary>
        /// Set the theme for the reCAPTCHA element. Does not have any effect when the size is set to <see cref="Size.Invisible"/>.
        /// </summary>
        public Theme Theme { get; set; } = Theme.Light;

        /// <summary>
        /// Set the size for the reCAPTCHA element. Please note that when you set the size to <see cref="Size.Invisible"/>, 
        /// you need to manually execute the reCAPTCHA.
        /// </summary>
        public Size Size { get; set; } = Size.Normal;

        /// <summary>
        /// Set the tabindex of the reCAPTCHA element. If other elements in your page use tabindex, it should be set to make user navigation easier.
        /// </summary>
        public int? TabIndex { get; set; } = null;

        /// <summary>
        /// Set the name of your callback function, executed when the user submits a successful response. The "g-recaptcha-response" token is passed to your callback.
        /// </summary>
        public string? Callback { get; set; }

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
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            _ = output ?? throw new ArgumentNullException(nameof(output));

            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;

            output.AddClass("g-recaptcha", HtmlEncoder.Default);

            output.Attributes.SetAttribute("data-sitekey", _settings.SiteKey);
            output.Attributes.SetAttribute("data-size", Size.ToString().ToLowerInvariant());
            output.Attributes.SetAttribute("data-theme", Theme.ToString().ToLowerInvariant());

            if (TabIndex != null)
            {
                output.Attributes.SetAttribute("data-tabindex", TabIndex);
            }

            if (!string.IsNullOrEmpty(Callback))
            {
                output.Attributes.SetAttribute("data-callback", Callback);
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
