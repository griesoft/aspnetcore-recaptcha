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
        /// The public reCAPTCHA site key. Will be added to reCAPTCHA HTML elements as the data-sitekey attribute.
        /// </summary>
        public string SiteKey { get; set; } = string.Empty;

        internal string SecretKey { get; set; } = string.Empty;
    }
}
