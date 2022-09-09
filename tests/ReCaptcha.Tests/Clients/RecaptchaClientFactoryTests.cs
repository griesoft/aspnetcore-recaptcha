using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using System;
using Griesoft.AspNetCore.ReCaptcha;
using Griesoft.AspNetCore.ReCaptcha.Clients;
using Griesoft.AspNetCore.ReCaptcha.Configuration;
using Microsoft.Extensions.Options;
using Moq.Protected;
using Moq;
using NUnit.Framework;

namespace ReCaptcha.Tests.Clients
{
    [TestFixture]
    public class RecaptchaClientFactoryTests
    {

        [Test]
        public void ClientCreation_WithProxy_IsSuccessful()
        {
            // Arrange
            var _settingsMock = new Mock<IOptionsMonitor<RecaptchaSettings>>();
            _settingsMock.SetupGet(instance => instance.CurrentValue)
                        .Returns(new RecaptchaSettings()
                        {
                            UseProxy = true,
                            ProxyAddress = "10.1.2.3:80"
                        })
                        .Verifiable();

            // Act
            var httpClientFactory = new RecaptchaClientFactory(_settingsMock.Object);
            var httpClient = httpClientFactory.CreateClient();

            // Assert //Can't access internal handler to check proxy settings
            Assert.AreEqual(RecaptchaServiceConstants.GoogleRecaptchaEndpoint, httpClient.BaseAddress.AbsoluteUri);
        }

        [Test]
        public void ClientCreation_WithoutProxy_IsSuccessful()
        {
            // Arrange
            var _settingsMock = new Mock<IOptionsMonitor<RecaptchaSettings>>();
            _settingsMock.SetupGet(instance => instance.CurrentValue)
                        .Returns(new RecaptchaSettings()
                        {
                            UseProxy = false
                        })
                        .Verifiable();

            // Act
            var httpClientFactory = new RecaptchaClientFactory(_settingsMock.Object);
            var httpClient = httpClientFactory.CreateClient();

            // Assert 
            Assert.AreEqual(RecaptchaServiceConstants.GoogleRecaptchaEndpoint, httpClient.BaseAddress.AbsoluteUri);
        }
    }
}
