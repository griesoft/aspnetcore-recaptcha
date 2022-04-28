namespace Griesoft.AspNetCore.ReCaptcha.Configuration
{
    /// <summary>
    /// Constant values for this service.
    /// </summary>
    public class RecaptchaServiceConstants
    {
        /// <summary>
        /// The validation endpoint.
        /// </summary>
        public const string GoogleRecaptchaEndpoint = "https://www.google.com/recaptcha/api/siteverify";

        /// <summary>
        /// The header key name under which the token is stored.
        /// </summary>
        public const string TokenKeyName = "G-Recaptcha-Response";

        /// <summary>
        /// The header key name under which the token is stored in lower case.
        /// </summary>
        public const string TokenKeyNameLower = "g-recaptcha-response";

        /// <summary>
        /// The section name in the appsettings.json from which the settings are read.
        /// </summary>
        public const string SettingsSectionKey = "RecaptchaSettings";
    }
}
