using System;
using Griesoft.AspNetCore.ReCaptcha;
using Griesoft.AspNetCore.ReCaptcha.Configuration;
using Griesoft.AspNetCore.ReCaptcha.Filters;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace ReCaptcha.Tests
{
    [TestFixture]
    public class ValidateRecaptchaAttributeTests
    {
        [Test(Description = "CreateInstance(...) should throw InvalidOperationException if the library services are not registered.")]
        public void CreateInstance_ShouldThrowWhen_ServicesNotRegistered()
        {
            // Arrange
            var servicesMock = new Mock<IServiceProvider>();
            servicesMock.Setup(provider => provider.GetService(typeof(ValidateRecaptchaFilter)))
                .Returns(null);
            var attribute = new ValidateRecaptchaAttribute();

            // Act


            // Assert
            Assert.Throws<InvalidOperationException>(() => attribute.CreateInstance(servicesMock.Object));
        }

        [Test(Description = "CreateInstance(...) should return a new instance of " +
            "ValidateRecaptchaFilter with the default value for the OnValidationFailedAction property.")]
        public void CreateInstance_ShouldReturn_ValidateRecaptchaFilter_WithDefaultOnValidationFailedAction()
        {
            // Arrange
            var optionsMock = new Mock<IOptionsMonitor<RecaptchaOptions>>();
            optionsMock.SetupGet(options => options.CurrentValue)
                .Returns(new RecaptchaOptions());
            var servicesMock = new Mock<IServiceProvider>();
            servicesMock.Setup(provider => provider.GetService(typeof(IValidateRecaptchaFilter)))
                .Returns(new ValidateRecaptchaFilter(null, optionsMock.Object, null))
                .Verifiable();
            var attribute = new ValidateRecaptchaAttribute();

            // Act
            var filterInstance = attribute.CreateInstance(servicesMock.Object);

            // Assert
            servicesMock.Verify();
            Assert.IsNotNull(filterInstance);
            Assert.IsInstanceOf<ValidateRecaptchaFilter>(filterInstance);
            Assert.AreEqual(ValidationFailedAction.Unspecified, (filterInstance as ValidateRecaptchaFilter).OnValidationFailedAction);
        }

        [Test(Description = "CreateInstance(...) should return a new instance of " +
            "ValidateRecaptchaFilter with the user set value for the OnValidationFailedAction property.")]
        public void CreateInstance_ShouldReturn_ValidateRecaptchaFilter_WithUserSetOnValidationFailedAction()
        {
            // Arrange
            var optionsMock = new Mock<IOptionsMonitor<RecaptchaOptions>>();
            optionsMock.SetupGet(options => options.CurrentValue)
                .Returns(new RecaptchaOptions());
            var servicesMock = new Mock<IServiceProvider>();
            servicesMock.Setup(provider => provider.GetService(typeof(IValidateRecaptchaFilter)))
                .Returns(new ValidateRecaptchaFilter(null, optionsMock.Object, null))
                .Verifiable();
            var attribute = new ValidateRecaptchaAttribute
            {
                ValidationFailedAction = ValidationFailedAction.ContinueRequest
            };

            // Act
            var filterInstance = attribute.CreateInstance(servicesMock.Object);

            // Assert
            servicesMock.Verify();
            Assert.IsNotNull(filterInstance);
            Assert.IsInstanceOf<ValidateRecaptchaFilter>(filterInstance);
            Assert.AreEqual(ValidationFailedAction.ContinueRequest, (filterInstance as ValidateRecaptchaFilter).OnValidationFailedAction);
        }

        [Test]
        public void CreateInstance_ShouldReturn_ValidateRecaptchaFilter_WithUserSetAction()
        {
            // Arrange
            var action = "submit";
            var optionsMock = new Mock<IOptionsMonitor<RecaptchaOptions>>();
            optionsMock.SetupGet(options => options.CurrentValue)
                .Returns(new RecaptchaOptions());
            var servicesMock = new Mock<IServiceProvider>();
            servicesMock.Setup(provider => provider.GetService(typeof(IValidateRecaptchaFilter)))
                .Returns(new ValidateRecaptchaFilter(null, optionsMock.Object, null))
                .Verifiable();
            var attribute = new ValidateRecaptchaAttribute
            {
                Action = action
            };

            // Act
            var filterInstance = attribute.CreateInstance(servicesMock.Object);

            // Assert
            servicesMock.Verify();
            Assert.IsNotNull(filterInstance);
            Assert.IsInstanceOf<ValidateRecaptchaFilter>(filterInstance);
            Assert.AreEqual(action, (filterInstance as ValidateRecaptchaFilter).Action);
        }
    }
}
