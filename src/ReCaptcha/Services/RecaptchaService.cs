using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GSoftware.AspNetCore.ReCaptcha.Configuration;
using GSoftware.AspNetCore.ReCaptcha.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

[assembly: InternalsVisibleTo("ReCaptcha.Tests")]
namespace GSoftware.AspNetCore.ReCaptcha.Services
{
    /// <inheritdoc />
    internal class RecaptchaService : IRecaptchaService
    {
        private readonly RecaptchaSettings _settings;
        private readonly HttpClient _httpClient;
        private readonly ILogger<RecaptchaService> _logger;

        public RecaptchaService(IOptionsMonitor<RecaptchaSettings> settings,
            HttpClient httpClient, ILogger<RecaptchaService> logger)
        {
            _settings = settings.CurrentValue;
            _httpClient = httpClient;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<ValidationResponse> ValidateRecaptchaResponse(string token, string? remoteIp = null)
        {
            _ = token ?? throw new ArgumentNullException(nameof(token));

            try
            {
                var response = await _httpClient.PostAsync($"?secret={_settings.SecretKey}&response={token}{(remoteIp != null ? $"&remoteip={remoteIp}" : "")}", null)
                    .ConfigureAwait(true);

                response.EnsureSuccessStatusCode();

                return JsonConvert.DeserializeObject<ValidationResponse>(
                    await response.Content.ReadAsStringAsync()
                    .ConfigureAwait(true));
            }
            catch (HttpRequestException)
            {
                _logger.LogWarning(Resources.RequestFailedErrorMessage);
                return new ValidationResponse()
                {
                    Success = false,
                    ErrorMessages = new List<string>()
                    {
                        "request-failed"
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, Resources.ValidationUnexpectedErrorMessage);
                throw;
            }
        }
    }
}
