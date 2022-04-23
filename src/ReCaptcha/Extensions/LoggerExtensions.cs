using System;
using Griesoft.AspNetCore.ReCaptcha.Localization;
using Microsoft.Extensions.Logging;

namespace Griesoft.AspNetCore.ReCaptcha.Extensions
{
    internal static class LoggerExtensions
    {
        private static readonly Action<ILogger, Exception?> _validationRequestFailed = LoggerMessage.Define(
            LogLevel.Warning,
            new EventId(1, nameof(ValidationRequestFailed)),
            Resources.RequestFailedErrorMessage);

        private static readonly Action<ILogger, Exception?> _validationRequestUnexpectedException = LoggerMessage.Define(
            LogLevel.Critical,
            new EventId(2, nameof(ValidationRequestUnexpectedException)),
            Resources.ValidationUnexpectedErrorMessage);

        private static readonly Action<ILogger, Exception?> _recaptchaResponseTokenMissing = LoggerMessage.Define(
            LogLevel.Warning,
            new EventId(3, nameof(RecaptchaResponseTokenMissing)),
            Resources.RecaptchaResponseTokenMissing);

        private static readonly Action<ILogger, Exception?> _invalidResponseToken = LoggerMessage.Define(
            LogLevel.Information,
            new EventId(4, nameof(InvalidResponseToken)),
            Resources.InvalidResponseTokenMessage);

        public static void ValidationRequestFailed(this ILogger logger)
        {
            _validationRequestFailed(logger, null);
        }

        public static void ValidationRequestUnexpectedException(this ILogger logger, Exception exception)
        {
            _validationRequestUnexpectedException(logger, exception);
        }

        public static void RecaptchaResponseTokenMissing(this ILogger logger)
        {
            _recaptchaResponseTokenMissing(logger, null);
        }

        public static void InvalidResponseToken(this ILogger logger)
        {
            _invalidResponseToken(logger, null);
        }
    }
}
