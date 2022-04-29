namespace Griesoft.AspNetCore.ReCaptcha.Configuration
{
    /// <summary>
    /// Mandatory settings for this reCAPTCHA service. The values for this object will be read from your appsettings.json file.
    /// </summary>
    /// <remarks>
    /// For more information about configuration in ASP.NET Core check out Microsoft docs: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-3.1
    /// </remarks>
    /// <seealso cref="Microsoft.Extensions.Configuration"/>
    public class RecaptchaSettings
    {
        /// <summary>
        /// The reCAPTCHA site key.
        /// </summary>
        /// <remarks>
        /// Will be added to reCAPTCHA HTML elements as the data-sitekey attribute.
        /// </remarks>
        public string SiteKey { get; set; } = string.Empty;

        /// <summary>
        /// The reCAPTCHA secret key.
        /// </summary>
        public string SecretKey { get; set; } = string.Empty;
    }
}
