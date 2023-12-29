using Newtonsoft.Json;

namespace WebAPI.Net6.BaseRepositoryGenerics.Infrastructure
{
    public partial class BaseResponse
    {

        /// <summary>
        /// Gets or sets the trace identifier.
        /// </summary>
        /// <value> The trace identifier. </value>
        [JsonProperty("traceId")]
        public string TraceId { get; set; } = Guid.NewGuid().ToString();
    }
}
