using Newtonsoft.Json;

namespace WebAPI.Net6.BaseRepositoryGenerics.Application.ErrorMessage.Common
{
    public class BaseAddAndUpdateResponse : BaseResponse
    {

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value> The request. </value>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the success.
        /// </summary>
        /// <value>
        /// The success.
        /// </value>
        [JsonProperty("success")]
        public bool? Success { get; set; } = true;

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        [JsonProperty("msg")]
        public string Msg { get; set; } = string.Empty;

    }
}
