using System;
using System.Text.Encodings.Web;
using Griesoft.AspNetCore.ReCaptcha.Configuration;
using Griesoft.AspNetCore.ReCaptcha.Extensions;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

namespace Griesoft.AspNetCore.ReCaptcha.TagHelpers
{
    /// <summary>
    /// A tag helper which adds a Google reCAPTCHA check box to your page.
    /// </summary>
    /// <remarks>
    /// If the reCAPTCHA element is outside of the form, the response token is not included in your form which will result in response verification failure.
    /// This can be prevented by either placing the reCAPTCHA inside your form or by using a callback function which will add the token to your form after the
    /// challenge was successfully completed.
    /// </remarks>
    ///  <example>
    /// The simplest use of the tag would be:
    /// <code>
    /// <recaptcha />
    /// </code>
    /// 
    /// Which will translate into the following HTML:
    /// <code>
    /// <div class="g-recaptcha" data-sitekey="your_site_key" data-size="normal" data-theme="light"></div>
    /// </code>
    /// </example>
    public class RecaptchaTagHelper : TagHelper
    {
        private readonly RecaptchaSettings _settings;
        private readonly RecaptchaOptions _options;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="options"></param>
        public RecaptchaTagHelper(IOptionsMonitor<RecaptchaSettings> settings, IOptionsMonitor<RecaptchaOptions> options)
        {
            _ = settings ?? throw new ArgumentNullException(nameof(settings));
            _ = options ?? throw new ArgumentNullException(nameof(options));

            _settings = settings.CurrentValue;
            _options = options.CurrentValue;

            Theme = _options.Theme;
            Size = _options.Size;
        }

        /// <summary>
        /// Set the theme for the reCAPTCHA element.
        /// </summary>
        /// <remarks>
        /// The invisible theme is not a option, because you should use <see cref="RecaptchaInvisibleTagHelper"/> instead for that.
        /// </remarks>
        public Theme Theme { get; set; }

        /// <summary>
        /// Set the size for the reCAPTCHA element.
        /// </summary>
        public Size Size { get; set; }

        /// <summary>
        /// Set the tabindex of the reCAPTCHA element. If other elements in your page use tabindex, it should be set to make user navigation easier.
        /// </summary>
        public int? TabIndex { get; set; }

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
        /// <exception cref="ArgumentNullException"></exception>
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
