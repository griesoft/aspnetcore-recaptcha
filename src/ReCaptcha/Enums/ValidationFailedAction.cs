namespace GSoftware.AspNetCore.ReCaptcha
{
    /// <summary>
    /// Options which specify what to do with a HTTP request when a reCAPTCHA response token was invalid.
    /// </summary>
    public enum ValidationFailedAction
    {
        Unspecified,
        BlockRequest,
        ContinueRequest
    }
}
