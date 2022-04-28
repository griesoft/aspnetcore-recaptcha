using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Griesoft.AspNetCore.ReCaptcha.Configuration;
using Griesoft.AspNetCore.ReCaptcha.Extensions;
using Griesoft.AspNetCore.ReCaptcha.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

[assembly: InternalsVisibleTo("ReCaptcha.Tests")]
namespace Griesoft.AspNetCore.ReCaptcha.Filters
{
    internal class ValidateRecaptchaFilter : IValidateRecaptchaFilter
    {
        private readonly IRecaptchaService _recaptchaService;
        private readonly RecaptchaOptions _options;
        private readonly ILogger<ValidateRecaptchaFilter> _logger;

        public ValidateRecaptchaFilter(IRecaptchaService recaptchaService, IOptionsMonitor<RecaptchaOptions> options,
            ILogger<ValidateRecaptchaFilter> logger)
        {
            _recaptchaService = recaptchaService;
            _logger = logger;
            _options = options.CurrentValue;
        }

        public ValidationFailedAction OnValidationFailedAction { get; set; } = ValidationFailedAction.Unspecified;

        public string? Action { get; set; }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (OnValidationFailedAction == ValidationFailedAction.Unspecified)
            {
                OnValidationFailedAction = _options.ValidationFailedAction;
            }

            ValidationResponse validationResponse;

            if (!TryGetRecaptchaToken(context.HttpContext.Request, out string? token))
            {
                _logger.RecaptchaResponseTokenMissing();

                validationResponse = new ValidationResponse()
                {
                    Success = false,
                    ErrorMessages = new List<string>()
                    {
                        "missing-input-response"
                    }
                };
            }
            else
            {
                validationResponse = await _recaptchaService.ValidateRecaptchaResponse(token, GetRemoteIp(context)).ConfigureAwait(true);
            }

            TryAddResponseToActionAguments(context, validationResponse);

            if (!ShouldShortCircuit(context, validationResponse))
            {
                await next.Invoke().ConfigureAwait(true);
            }
        }

        private string? GetRemoteIp(ActionExecutingContext context)
        {
            return _options.UseRemoteIp ?
                context.HttpContext.Connection.RemoteIpAddress?.ToString() :
                null;
        }
        private bool ShouldShortCircuit(ActionExecutingContext context, ValidationResponse response)
        {
            if (!response.Success || Action != response.Action)
            {
                _logger.InvalidResponseToken();

                if (OnValidationFailedAction == ValidationFailedAction.BlockRequest)
                {
                    context.Result = new RecaptchaValidationFailedResult();
                    return true;
                }
            }

            return false;
        }
        private static bool TryGetRecaptchaToken(HttpRequest request, [NotNullWhen(true)] out string? token)
        {
            if (request.Headers.ContainsKey(RecaptchaServiceConstants.TokenKeyName))
            {
                token = request.Headers[RecaptchaServiceConstants.TokenKeyName];
            }
            else if (request.HasFormContentType && request.Form.ContainsKey(RecaptchaServiceConstants.TokenKeyNameLower))
            {
                token = request.Form[RecaptchaServiceConstants.TokenKeyNameLower];
            }
            else if (request.Query.ContainsKey(RecaptchaServiceConstants.TokenKeyNameLower))
            {
                token = request.Query[RecaptchaServiceConstants.TokenKeyNameLower];
            }
            else
            {
                token = null;
            }

            return token != null;
        }
        private static void TryAddResponseToActionAguments(ActionExecutingContext context, ValidationResponse response)
        {
            if (context.ActionArguments.Any(pair => pair.Value is ValidationResponse))
            {
                context.ActionArguments[context.ActionArguments.First(pair => pair.Value is ValidationResponse).Key] = response;
            }
        }
    }
}
