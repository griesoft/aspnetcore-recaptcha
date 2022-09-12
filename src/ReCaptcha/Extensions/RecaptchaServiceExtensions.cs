using System;
using System.Net;
using System.Net.Http;
using Griesoft.AspNetCore.ReCaptcha.Configuration;
using Griesoft.AspNetCore.ReCaptcha.Filters;
using Griesoft.AspNetCore.ReCaptcha.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// <see cref="IServiceCollection"/> extension methods for easy service registration in the StartUp.cs.
    /// </summary>
    public static class RecaptchaServiceExtensions
    {
        /// <summary>
        /// Register the <see cref="RecaptchaService"/> to the web project and all it's dependencies.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options">Specify global options for the service.</param>
        /// <returns></returns>
        public static IServiceCollection AddRecaptchaService(this IServiceCollection services, Action<RecaptchaOptions>? options = null)
        {
            services.AddOptions<RecaptchaSettings>()
                    .Configure<IConfiguration>((settings, config) =>
                     config.GetSection(RecaptchaServiceConstants.SettingsSectionKey)
                    .Bind(settings, (op) => op.BindNonPublicProperties = true));

            services.Configure(options ??= opt => { });

            //build temporary service provider to access settings
            var serviceProvider = services.BuildServiceProvider();
            var settings = serviceProvider.GetRequiredService<IOptions<RecaptchaSettings>>()?.Value;

            //register http client with recaptcha base address
            var httpClientBuilder = services.AddHttpClient(RecaptchaServiceConstants.RecaptchaServiceHttpClientName, client =>
            {
                client.BaseAddress = new Uri(RecaptchaServiceConstants.GoogleRecaptchaEndpoint);
            });

            //if necessary use handler that utilizes a proxy
            if (settings?.UseProxy == true) {
                httpClientBuilder.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
                 {
                     UseProxy = true,
                     Proxy = new WebProxy(settings.ProxyAddress, false)
                 });
            }

            services.AddScoped<IRecaptchaService, RecaptchaService>();

            services.AddTransient<IValidateRecaptchaFilter, ValidateRecaptchaFilter>();

            return services;
        }
    }
}
