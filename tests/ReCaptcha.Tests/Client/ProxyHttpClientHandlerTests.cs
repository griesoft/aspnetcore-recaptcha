using Griesoft.AspNetCore.ReCaptcha;
using Griesoft.AspNetCore.ReCaptcha.Client;
using Griesoft.AspNetCore.ReCaptcha.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace ReCaptcha.Tests.Client
{
    [TestFixture]
    public class ProxyHttpClientHandlerTests
    {
        [Test]
        public void Initialize_WithProxy()
        {
            // Arrange
            var settingsMock = new Mock<IOptions<RecaptchaSettings>>();
            settingsMock.SetupGet(instance => instance.Value)
                .Returns(new RecaptchaSettings()
                {
                    UseProxy = true,
                    ProxyAddress = "http://10.1.2.3:80"
                });

            // Act
            var handler = new ProxyHttpClientHandler(settingsMock.Object);

            // Assert
            Assert.IsTrue(handler.UseProxy);
            Assert.IsNotNull(handler.Proxy);
        }

        [Test]
        public void Initialize_NoProxy()
        {
            // Arrange
            var settingsMock = new Mock<IOptions<RecaptchaSettings>>();
            settingsMock.SetupGet(instance => instance.Value)
                .Returns(new RecaptchaSettings()
                {
                    UseProxy = false
                });

            // Act
            var handler = new ProxyHttpClientHandler(settingsMock.Object);

            // Assert
            Assert.IsFalse(handler.UseProxy);
            Assert.IsNull(handler.Proxy);
        }

        [Test]
        public void Initialize_NotSpecified()
        {
            // Arrange
            var settingsMock = new Mock<IOptions<RecaptchaSettings>>();
            settingsMock.SetupGet(instance => instance.Value)
                .Returns(new RecaptchaSettings()
                {
                });

            // Act
            var handler = new ProxyHttpClientHandler(settingsMock.Object);

            // Assert
            Assert.IsFalse(handler.UseProxy);
            Assert.IsNull(handler.Proxy);
        }
    }
}
