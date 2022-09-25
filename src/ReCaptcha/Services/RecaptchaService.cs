using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Griesoft.AspNetCore.ReCaptcha.Configuration;
using Griesoft.AspNetCore.ReCaptcha.Extensions;
using Griesoft.AspNetCore.ReCaptcha.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

[assembly: InternalsVisibleTo("ReCaptcha.Tests")]
namespace Griesoft.AspNetCore.ReCaptcha.Services
{
    /// <inheritdoc />
    internal class RecaptchaService : IRecaptchaService
    {
        private readonly RecaptchaSettings _settings;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<RecaptchaService> _logger;

        public RecaptchaService(IOptionsMonitor<RecaptchaSettings> settings,
            IHttpClientFactory httpClientFactory, ILogger<RecaptchaService> logger)
        {
            _settings = settings.CurrentValue;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        /// <inheritdoc />
        public ValidationResponse? ValidationResponse { get; private set; }

        /// <inheritdoc />
        public async Task<ValidationResponse> ValidateRecaptchaResponse(string token, string? remoteIp = null)
        {
            _ = token ?? throw new ArgumentNullException(nameof(token));

            try
            {
                var httpClient = _httpClientFactory.CreateClient(RecaptchaServiceConstants.RecaptchaServiceHttpClientName);
                var response = await httpClient.PostAsync($"?secret={_settings.SecretKey}&response={token}{(remoteIp != null ? $"&remoteip={remoteIp}" : "")}", null!)
                    .ConfigureAwait(true);

                response.EnsureSuccessStatusCode();

                ValidationResponse = JsonConvert.DeserializeObject<ValidationResponse>(
                    await response.Content.ReadAsStringAsync()
                    .ConfigureAwait(true))
                    ?? new ValidationResponse()
                    {
                        Success = false,
                        ErrorMessages = new List<string>()
                        {
                            "response-deserialization-failed"
                        }
                    };
            }
            catch (HttpRequestException)
            {
                _logger.ValidationRequestFailed();
                ValidationResponse = new ValidationResponse()
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
                _logger.ValidationRequestUnexpectedException(ex);
                throw;
            }

            return ValidationResponse;
        }
    }
}

