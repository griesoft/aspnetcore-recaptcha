using System.Net.Http;

namespace Griesoft.AspNetCore.ReCaptcha.Clients
{
    /// <summary>
    /// Constructs http client used for verifying captcha result with Google
    /// </summary>
    public interface IRecaptchaHttpClientFactory
    {
        /// <summary>
        /// Create HttpClient with preconfigured options to communicate with Google reCatpcha REST service
        /// </summary>
        /// <returns></returns>
        HttpClient CreateClient();
    }
}
