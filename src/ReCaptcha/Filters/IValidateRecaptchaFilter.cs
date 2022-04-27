using Microsoft.AspNetCore.Mvc.Filters;

namespace Griesoft.AspNetCore.ReCaptcha.Filters
{
    /// <summary>
    /// An action filter which does validate that the request contains a valid reCAPTCHA token.
    /// </summary>
    public interface IValidateRecaptchaFilter : IAsyncActionFilter
    {
        /// <summary>
        /// The action that the filter should take when validation of a token fails.
        /// </summary>
        public ValidationFailedAction OnValidationFailedAction { get; set; }

        /// <summary>
        /// The reCAPTCHA V3 action name.
        /// </summary>
        /// <remarks>
        /// This will also be validated for a matching action name in the validation response from the reCAPTCHA service.
        /// </remarks>
        public string? Action { get; set; }
    }
}
