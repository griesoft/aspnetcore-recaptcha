using System;
using Griesoft.AspNetCore.ReCaptcha.Clients;
using Griesoft.AspNetCore.ReCaptcha.Configuration;
using Griesoft.AspNetCore.ReCaptcha.Filters;
using Griesoft.AspNetCore.ReCaptcha.Services;
using Microsoft.Extensions.Configuration;

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

            services.AddScoped<IRecaptchaHttpClientFactory, RecaptchaClientFactory>();
            services.AddScoped<IRecaptchaService, RecaptchaService>();

            services.AddTransient<IValidateRecaptchaFilter, ValidateRecaptchaFilter>();

            return services;
        }
    }
}
