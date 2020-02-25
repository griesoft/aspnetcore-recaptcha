using Microsoft.AspNetCore.Mvc;

namespace Griesoft.AspNetCore.ReCaptcha.Filters
{
    /// <summary>
    /// Represents an <see cref="IActionResult"/> that is used when the recaptcha validation failed. 
    /// This can be matched inside MVC result filters to process the validation failure.
    /// </summary>
    public interface IRecaptchaValidationFailedResult : IActionResult
    {
    }
}
