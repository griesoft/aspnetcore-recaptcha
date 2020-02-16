using System;
using System.Collections.Generic;
using GSoftware.AspNetCore.ReCaptcha.Configuration;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace GSoftware.AspNetCore.ReCaptcha.TagHelpers
{
    /// <summary>
    /// Adds a script tag, which will load the required reCAPTCHA JavaScript API. Can be added
    /// anywhere on your HTML page, but if you use a onload callback function you must place this
    /// after that callback to avoid race conditions.
    /// </summary>
    public class RecaptchaScriptTagHelper : TagHelper
    {
        private const string RecaptchaScriptEndpoint = "https://www.google.com/recaptcha/api.js";

        private readonly RecaptchaSettings _settings;

        public RecaptchaScriptTagHelper(IOptionsMonitor<RecaptchaSettings> settings)
        {
            _ = settings ?? throw new ArgumentNullException(nameof(settings));

            _settings = settings.CurrentValue;
        }

        /// <summary>
        /// Set the rendering mode for the script.
        /// </summary>
        public Render Render { get; set; } = Render.Onload;

        /// <summary>
        /// Set a callback function that will be called when reCAPTCHA has finished loading.
        /// </summary>
        public string? OnloadCallback { get; set; }

        /// <summary>
        /// Set a language code to force reCAPTCHA loading in the specified language. If not set language
        /// will be detected automatically by reCAPTCHA. For a list of valid language codes visit: 
        /// https://developers.google.com/recaptcha/docs/language
        /// </summary>
        public string? Language { get; set; }

        /// <inheritdoc/>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            _ = output ?? throw new ArgumentNullException(nameof(output));

            output.TagName = "script";

            if (Render == Render.V3)
            {
                output.Attributes.Add("src", $"{RecaptchaScriptEndpoint}?render={_settings.SiteKey}");
            }
            else
            {
                var queryCollection = new Dictionary<string, string>();

                if (!string.IsNullOrEmpty(OnloadCallback))
                {
                    queryCollection.Add("onload", OnloadCallback);
                }

                if (Render == Render.Explicit)
                {
                    queryCollection.Add("render", Render.ToString().ToLowerInvariant());
                }

                if (!string.IsNullOrEmpty(Language))
                {
                    queryCollection.Add("hl", Language);
                }

                output.Attributes.Add("src", QueryHelpers.AddQueryString(RecaptchaScriptEndpoint, queryCollection));
            }

            output.Attributes.Add("async", null);
            output.Attributes.Add("defer", null);
        }
    }
}
