using System;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using Griesoft.AspNetCore.ReCaptcha.Configuration;
using Microsoft.Extensions.Options;

[assembly: InternalsVisibleTo("ReCaptcha.Tests")]
namespace Griesoft.AspNetCore.ReCaptcha.Clients
{
    internal class RecaptchaClientFactory : IRecaptchaHttpClientFactory
    {
        private readonly RecaptchaOptions _options;

        public RecaptchaClientFactory(IOptionsMonitor<RecaptchaOptions> options)
        {
            _options = options.CurrentValue;
        }


        public HttpClient CreateClient()
        {
            var httpClientHandler = new HttpClientHandler();
            if (_options.UseProxy.HasValue && _options.UseProxy.Value && !String.IsNullOrEmpty(_options.ProxyAddress))
            {
                httpClientHandler.UseProxy = true;
                httpClientHandler.Proxy = new WebProxy(_options.ProxyAddress, false);
            }

            var httpClient = new HttpClient(httpClientHandler)
            {
                BaseAddress = new Uri(RecaptchaServiceConstants.GoogleRecaptchaEndpoint)
            };

            return httpClient;
        }

    }
}
