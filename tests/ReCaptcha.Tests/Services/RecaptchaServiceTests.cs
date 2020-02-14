using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GSoftware.AspNetCore.ReCaptcha;
using GSoftware.AspNetCore.ReCaptcha.Configuration;
using GSoftware.AspNetCore.ReCaptcha.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace ReCaptcha.Tests.Services
{
    [TestFixture]
    public class RecaptchaServiceTests
    {
        private const string SiteKey = "sitekey";
        private const string SecretKey = "verysecretkey";
        private const string Token = "testtoken";

        private ILogger<RecaptchaService> _logger;
        private Mock<IOptionsMonitor<RecaptchaSettings>> _optionsMock;
        private Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private HttpClient _httpClient;

        [SetUp]
        public void Initialize()
        {
            _logger = new LoggerFactory().CreateLogger<RecaptchaService>();

            _optionsMock = new Mock<IOptionsMonitor<RecaptchaSettings>>();
            _optionsMock.SetupGet(instance => instance.CurrentValue)
                .Returns(new RecaptchaSettings()
                {
                    SiteKey = SiteKey,
                    SecretKey = SecretKey
                })
                .Verifiable();

            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpMessageHandlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{'success': true, 'challenge_ts': '" + DateTime.UtcNow.ToString("o") + "', 'hostname': 'https://test.com', 'error-codes': []}")
               })
               .Verifiable();

            _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("http://test.com/"),
            };
        }

        [Test]
        public void ValidateRecaptchaResponse_ShouldThrow_ArgumentNullException()
        {
            // Arrange
            var service = new RecaptchaService(_optionsMock.Object, _httpClient, _logger);

            // Act


            // Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => service.ValidateRecaptchaResponse(null));
        }

        [Test]
        public async Task ValidateRecaptchaResponse_ShouldReturn_HttpRequestError()
        {
            // Arrange
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpMessageHandlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.BadRequest
               })
               .Verifiable();

            _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("http://test.com/"),
            };

            var service = new RecaptchaService(_optionsMock.Object, _httpClient, _logger);

            // Act
            var response = await service.ValidateRecaptchaResponse(Token);

            // Assert
            _httpMessageHandlerMock.Verify();
            Assert.GreaterOrEqual(response.Errors.Count(), 1);
            Assert.AreEqual(ValidationError.HttpRequestFailed, response.Errors.First());
        }

        [Test]
        public void ValidateRecaptchaResponse_ShouldThrowAnyOtherThan_HttpRequestException()
        {
            // Arrange
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpMessageHandlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               .ThrowsAsync(new Exception())
               .Verifiable();

            _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("http://test.com/"),
            };

            var service = new RecaptchaService(_optionsMock.Object, _httpClient, _logger);

            // Act


            // Assert
            Assert.ThrowsAsync<Exception>(() => service.ValidateRecaptchaResponse(Token));
            _httpMessageHandlerMock.Verify();
        }

        [Test]
        public async Task ValidateRecaptchaResponse_ShouldReturn_DeserializedResponse()
        {
            // Arrange
            var service = new RecaptchaService(_optionsMock.Object, _httpClient, _logger);

            // Act
            var response = await service.ValidateRecaptchaResponse(Token);

            // Assert
            _httpMessageHandlerMock.Verify();
            Assert.IsTrue(response.Success);
            Assert.AreEqual(0, response.Errors.Count());
        }
    }
}
