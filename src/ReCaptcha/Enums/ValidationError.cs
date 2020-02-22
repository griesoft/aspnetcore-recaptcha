namespace GSoftware.AspNetCore.ReCaptcha
{
    /// <summary>
    /// Recaptcha validation error reason message enum fields.
    /// </summary>
    public enum ValidationError
    {
        /// <summary>
        /// Somethign went wrong in a very bad way. You can consider yourself lucky when you hit this error.
        /// </summary>
        Undefined,

        /// <summary>
        /// No input secret was provided. Make sure you have configured the service correctly.
        /// </summary>
        MissingInputSecret,

        /// <summary>
        /// The secret parameter is invalid or malformed. Make sure you have not switched the secret key with the site key accidently.
        /// </summary>
        InvalidInputSecret,

        /// <summary>
        /// The response token is missing.
        /// </summary>
        MissingInputResponse,

        /// <summary>
        /// The response parameter is invalid or malformed.
        /// </summary>
        InvalidInputResponse,

        /// <summary>
        /// The request is invalid or malformed.
        /// </summary>
        BadRequest,

        /// <summary>
        /// The response is no longer valid: either is too old or has been used previously.
        /// </summary>
        TimeoutOrDuplicate,

        /// <summary>
        /// The connection to the reCAPTCHA validation endpoint failed.
        /// </summary>
        HttpRequestFailed
    }
}
