using System;
using System.Diagnostics;
using System.Collections.Generic;
using JsonApiDotNetCore.Configuration;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

namespace JsonApiDotNetCore.Internal
{
    public class Error
    {
        public Error() { }

        public Error(int status, string title, ErrorMeta meta = null, object source = null)
        {
            Status = status.ToString();
            Title = title;
            Meta = meta;
            Source = source;
        }

        public Error(int status, string title, string detail, ErrorMeta meta = null, object source = null)
        {
            Status = status.ToString();
            Title = title;
            Detail = detail;
            Meta = meta;
            Source = source;
        }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("detail")]
        public string Detail { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonIgnore]
        public int StatusCode => int.Parse(Status);

        [JsonProperty("source")]
        public object Source { get; set; }

        [JsonProperty("meta")]
        public ErrorMeta Meta { get; set; }

        public bool ShouldSerializeMeta() => (JsonApiOptions.DisableErrorStackTraces == false);
        public bool ShouldSerializeSource() => (JsonApiOptions.DisableErrorSource == false);

        public IActionResult AsActionResult()
        {
            var errorCollection = new ErrorCollection
            {
                Errors = new List<Error> { this }
            };

            return errorCollection.AsActionResult();
        }
    }

    public class ErrorMeta
    {
        [JsonProperty("stackTrace")]
        public string[] StackTrace { get; set; }

        public static ErrorMeta FromException(Exception e)
            => new ErrorMeta {
                StackTrace = e.Demystify().ToString().Split(new[] { "\n"}, int.MaxValue, StringSplitOptions.RemoveEmptyEntries)
            };
    }
}
