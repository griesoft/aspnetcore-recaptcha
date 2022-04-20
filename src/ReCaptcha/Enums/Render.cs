using System;

namespace Griesoft.AspNetCore.ReCaptcha.TagHelpers
{
    /// <summary>
    /// Recaptcha rendering options for the <see cref="RecaptchaScriptTagHelper"/>.
    /// </summary>
    [Flags]
    public enum Render
    {
        /// <summary>
        /// The default rendering option. This will render your V2 reCAPTCHA elements automatically after the script has been loaded.
        /// </summary>
        Onload,

        /// <summary>
        /// When rendering your reCAPTCHA elements explicitly a given onloadCallback will be called after the script has been loaded.
        /// </summary>
        Explicit,

        /// <summary>
        /// Loads the reCAPTCHA V3 script.
        /// </summary>
        V3
    }
}
