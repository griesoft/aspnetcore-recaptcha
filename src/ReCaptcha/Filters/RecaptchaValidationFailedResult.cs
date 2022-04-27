using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Griesoft.AspNetCore.ReCaptcha.Filters
{
    /// <summary>
    /// A bad request result used for reCAPTCHA validation failures. Use <see cref="IRecaptchaValidationFailedResult"/> to
    /// match for validation failures inside MVC result filters.
    /// </summary>
    public class RecaptchaValidationFailedResult : IActionResult, IRecaptchaValidationFailedResult
    {
        /// <inheritdoc />
        public Task ExecuteResultAsync(ActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.HttpContext.Response.StatusCode = 400;

            return Task.CompletedTask;
        }
    }
}
