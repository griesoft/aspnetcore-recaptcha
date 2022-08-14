using System;
using System.Threading.Tasks;

namespace Griesoft.AspNetCore.ReCaptcha.Services
{
    /// <summary>
    /// A service for reCAPTCHA response back-end validation.
    /// </summary>
    public interface IRecaptchaService
    {
        /// <summary>
        /// Access the validation response of the last validation that this service did perform.
        /// </summary>
        /// <remarks>
        /// This service is registered as transient (or should be) which means the validation response will
        /// always match the request that instantiated this service.
        /// </remarks>
        ValidationResponse? ValidationResponse { get; }

        /// <summary>
        /// Validate the reCAPTCHA response token.
        /// </summary>
        /// <param name="token">The response token.</param>
        /// <param name="remoteIp">If set, the remote IP will be send to Google for validation too.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        Task<ValidationResponse> ValidateRecaptchaResponse(string token, string? remoteIp = null);
    }
}
