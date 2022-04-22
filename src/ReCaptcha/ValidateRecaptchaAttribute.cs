using System;
using Griesoft.AspNetCore.ReCaptcha.Filters;
using Griesoft.AspNetCore.ReCaptcha.Localization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Griesoft.AspNetCore.ReCaptcha
{
    /// <summary>
    /// Validates an incoming request that it contains a valid ReCaptcha token.
    /// </summary>
    /// <remarks>
    /// Can be applied to a specific action or to a controller which would validate all incoming requests to it.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class ValidateRecaptchaAttribute : Attribute, IFilterFactory, IOrderedFilter
    {
        /// <inheritdoc />
        public int Order { get; }

        /// <inheritdoc />
        public bool IsReusable { get; }

        /// <summary>
        /// If set to <see cref="ValidationFailedAction.BlockRequest"/>, the requests that do not contain a valid reCAPTCHA response token will be canceled. 
        /// If this is set to anything else than <see cref="ValidationFailedAction.Unspecified"/>, this will override the global behavior.
        /// </summary>
        public ValidationFailedAction ValidationFailedAction { get; set; } = ValidationFailedAction.Unspecified;

        /// <summary>
        /// The name of the action that is verified.
        /// </summary>
        /// <remarks>
        /// This is a reCAPTCHA V3 feature and should be used only when validating V3 challenges.
        /// </remarks>
        public string? Action { get; set; }


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
            filter.Action = Action;

            return filter;
        }
    }
}
