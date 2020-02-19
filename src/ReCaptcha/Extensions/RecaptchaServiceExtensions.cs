using System;
using GSoftware.AspNetCore.ReCaptcha.Configuration;
using GSoftware.AspNetCore.ReCaptcha.Filters;
using GSoftware.AspNetCore.ReCaptcha.Services;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RecaptchaServiceExtensions
    {
        /// <summary>
        /// Register the <see cref="RecaptchaService"/> to the web project and all it's dependencies.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddRecaptchaService(this IServiceCollection services, Action<RecaptchaOptions>? options = null)
        {
            services.AddOptions<RecaptchaSettings>()
                .Configure<IConfiguration>((settings, config) =>
                    config.GetSection(RecaptchaServiceConstants.SettingsSectionKey)
                    .Bind(settings, (op) => op.BindNonPublicProperties = true));

            services.Configure(options ??= opt => { });

            services.AddHttpClient<IRecaptchaService, RecaptchaService>(client =>
            {
                client.BaseAddress = new Uri(RecaptchaServiceConstants.GoogleRecaptchaEndpoint);
            });

            services.AddTransient<ValidateRecaptchaFilter>();

            return services;
        }
    }
}
