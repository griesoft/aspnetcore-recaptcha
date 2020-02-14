namespace GSoftware.AspNetCore.ReCaptcha
{
    /// <summary>
    /// Recaptcha validation error reason message enum fields.
    /// </summary>
    public enum ValidationError
    {
        Undefined,
        MissingInputSecret,
        InvalidInputSecret,
        MissingInputResponse,
        InvalidInputResponse,
        BadRequest,
        TimeoutOrDuplicate,
        HttpRequestFailed
    }
}
