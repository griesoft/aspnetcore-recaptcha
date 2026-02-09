using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

[assembly: InternalsVisibleTo("ReCaptcha.Tests")]
namespace Griesoft.AspNetCore.ReCaptcha
{
    /// <summary>
    /// Recaptcha validation response model.
    /// </summary>
    public class ValidationResponse
    {
        /// <summary>
        /// Validation success status.
        /// </summary>
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        /// <summary>
        /// The score for this request (0.0 - 1.0). Only used with reCAPTCHA V3.
        /// </summary>
        [JsonPropertyName("score")]
        public double? Score { get; set; } = null;

        /// <summary>
        /// The action name for this request (important to verify). Only used with reCAPTCHA V3.
        /// </summary>
        [JsonPropertyName("action")]
        public string? Action { get; set; } = null;

        /// <summary>
        /// Time stamp of the challenge load.
        /// </summary>
        [JsonPropertyName("challenge_ts")]
        public DateTime ChallengeTimeStamp { get; set; }

        /// <summary>
        /// The host name of the site where the reCAPTCHA was solved.
        /// </summary>
        [JsonPropertyName("hostname")]
        public string Hostname { get; set; } = string.Empty;

        /// <summary>
        /// List of <see cref="ValidationError"/>'s, if any occurred.
        /// </summary>
        [JsonIgnore]
        public IEnumerable<ValidationError> Errors => GetValidationErrors();

        [JsonPropertyName("error-codes")]
        internal List<string> ErrorMessages { get; set; } = [];

        private IEnumerable<ValidationError> GetValidationErrors()
        {
            foreach (var s in ErrorMessages)
            {
                yield return s switch
                {
                    "missing-input-secret" => ValidationError.MissingInputSecret,
                    "invalid-input-secret" => ValidationError.InvalidInputSecret,
                    "missing-input-response" => ValidationError.MissingInputResponse,
                    "invalid-input-response" => ValidationError.InvalidInputResponse,
                    "bad-request" => ValidationError.BadRequest,
                    "timeout-or-duplicate" => ValidationError.TimeoutOrDuplicate,
                    "request-failed" => ValidationError.HttpRequestFailed,
                    _ => ValidationError.Undefined
                };
            }
        }
    }
}
