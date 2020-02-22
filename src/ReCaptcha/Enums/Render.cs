namespace GSoftware.AspNetCore.ReCaptcha.TagHelpers
{
    /// <summary>
    /// Recaptcha rendering options for the <see cref="RecaptchaScriptTagHelper"/>.
    /// </summary>
    public enum Render
    {
        /// <summary>
        /// Render the reCAPTCHA elements explicitly after the scripts have loaded successfully.
        /// You need to provide a success callback and handle rendering of the reCAPTCHA elements in it yourself.
        /// </summary>
        Explicit,

        /// <summary>
        /// The default rendering option. This will render your reCAPTCHA elements automatically after the script has loaded successfully.
        /// </summary>
        Onload,

        /// <summary>
        /// Loads the reCAPTCHA V3 script. It will behave mostly the same like <see cref="Explicit"/>, so you have to render the reCAPTCHA elements yourself.
        /// </summary>
        V3
    }
}
