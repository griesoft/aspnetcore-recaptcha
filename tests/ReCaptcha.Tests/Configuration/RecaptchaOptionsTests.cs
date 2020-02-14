using GSoftware.AspNetCore.ReCaptcha;
using GSoftware.AspNetCore.ReCaptcha.Configuration;
using NUnit.Framework;

namespace ReCaptcha.Tests.Configuration
{
    [TestFixture]
    public class RecaptchaOptionsTests
    {
        [Test]
        public void ValidationFailedAction_ShouldNeverReturn_ValidationFailedActionUnspecified()
        {
            // Arrange
            var optionsUnmodified = new RecaptchaOptions();
            var optionsModified = new RecaptchaOptions();

            // Act
            optionsModified.ValidationFailedAction = ValidationFailedAction.Unspecified;

            // Assert
            Assert.AreNotEqual(ValidationFailedAction.Unspecified, optionsUnmodified.ValidationFailedAction);
            Assert.AreNotEqual(ValidationFailedAction.Unspecified, optionsModified.ValidationFailedAction);
        }
    }
}
