using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using Griesoft.AspNetCore.ReCaptcha.Configuration;
using Microsoft.Extensions.Options;

namespace Griesoft.AspNetCore.ReCaptcha.Client
{

    /// <summary>
    /// HttpClientHandler utilizing proxy server if such configuration was provided
    /// </summary>
    public class ProxyHttpClientHandler : HttpClientHandler
    {
        /// <summary>
        /// Create HttpHandler configured by RecaptchaSettings
        /// </summary>
        /// <param name="settings">Recaptcha Settings sepcifying proxy configuration</param>
        public ProxyHttpClientHandler(IOptions<RecaptchaSettings> settings)
        {
            var currentSettings = settings?.Value;

            if (currentSettings != null && currentSettings.UseProxy == true && !String.IsNullOrEmpty(currentSettings.ProxyAddress))
            {
                this.UseProxy = true;
                this.Proxy = new WebProxy(currentSettings.ProxyAddress, currentSettings.BypassOnLocal);
            }
            else
            {
                this.UseProxy = false;
            }
        }
    }
}
