using System;
using GSoftware.AspNetCore.ReCaptcha.Filters;
using GSoftware.AspNetCore.ReCaptcha.Localization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace GSoftware.AspNetCore.ReCaptcha
{
    /// <summary>
    /// Validates an incoming POST request to a controller or action, which is decorated with this attribute 
    /// that the header contains a valid ReCaptcha token. If the token is missing or is not valid, the action
    /// will not be executed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class ValidateRecaptchaAttribute : Attribute, IFilterFactory, IOrderedFilter
    {
        /// <inheritdoc />
        public int Order { get; }

        /// <inheritdoc />
        public bool IsReusable { get; }

        /// <summary>
        /// If set to <see cref="ValidationFailedAction.BlockRequest"/>, the requests that do not contain a valid reCAPTCHA response token will be canceled. 
        /// If this is set to anything else than <see cref="ValidationFailedAction.Unspecified"/>, this will override the global behaviour, 
        /// which you might have set at app startup.
        /// </summary>
        public ValidationFailedAction ValidationFailedAction { get; set; } = ValidationFailedAction.Unspecified;


        /// <summary>
        /// Creates an instance of the executable filter.
        /// </summary>
        /// <param name="serviceProvider">The request <see cref="IServiceProvider"/>.</param>
        /// <returns>An instance of the executable filter.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the required services ar not registered.</exception>
        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
#pragma warning disable CA1062 // Validate arguments of public methods
            var filter = serviceProvider.GetService(typeof(ValidateRecaptchaFilter)) as ValidateRecaptchaFilter;
#pragma warning restore CA1062 // Validate arguments of public methods

            _ = filter ?? throw new InvalidOperationException(Resources.RequiredServiceNotRegisteredErrorMessage);

            filter.OnValidationFailedAction = ValidationFailedAction;

            return filter;
        }
    }
}
