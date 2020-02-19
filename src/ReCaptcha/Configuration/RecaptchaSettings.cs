namespace GSoftware.AspNetCore.ReCaptcha.Configuration
{
    public class RecaptchaSettings
    {
        public string SiteKey { get; set; } = string.Empty;

        internal string SecretKey { get; set; } = string.Empty;
    }
}
