using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ReCaptcha.Tests")]
namespace Griesoft.AspNetCore.ReCaptcha.Configuration
{
    internal class RecaptchaServiceConstants
    {
        internal const string GoogleRecaptchaEndpoint = "https://www.google.com/recaptcha/api/siteverify";
        internal const string TokenKeyName = "G-Recaptcha-Response";
        internal const string TokenKeyNameLower = "g-recaptcha-response";
        internal const string SettingsSectionKey = "RecaptchaSettings";
    }
}
