namespace GSoftware.AspNetCore.ReCaptcha
{
    /// <summary>
    /// Options which specify what to do with a HTTP request when a reCAPTCHA response token was invalid.
    /// </summary>
    public enum ValidationFailedAction
    {
        /// <summary>
        /// 
        /// </summary>
        Unspecified,

        /// <summary>
        /// The validation filter will block and stop execution of requests which did fail reCAPTCHA response verification.
        /// </summary>
        BlockRequest,

        /// <summary>
        /// The validation filter will allow the request to continue even if the reCAPTCHA response verification failed.
        /// </summary>
        ContinueRequest
    }
}
