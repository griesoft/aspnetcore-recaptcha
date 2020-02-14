using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

[assembly: InternalsVisibleTo("ReCaptcha.Tests")]
namespace GSoftware.AspNetCore.ReCaptcha
{
    /// <summary>
    /// Recaptcha validation response model.
    /// </summary>
    public class ValidationResponse
    {
        /// <summary>
        /// Validation success status.
        /// </summary>
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }

        /// <summary>
        /// Timestamp of the challenge load.
        /// </summary>
        [JsonProperty(PropertyName = "challenge_ts")]
        public DateTime ChallengeTimeStamp { get; set; }

        /// <summary>
        /// The hostname of the site where the reCAPTCHA was solved.
        /// </summary>
        [JsonProperty(PropertyName = "hostname")]
        public string Hostname { get; set; } = string.Empty;

        /// <summary>
        /// List of <see cref="ValidationError"/>'s, if any occured.
        /// </summary>
        [JsonIgnore]
        public IEnumerable<ValidationError> Errors => GetValidationErrors();

        [JsonProperty(PropertyName = "error-codes")]
        internal List<string> ErrorMessages { get; set; } = new List<string>();

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
