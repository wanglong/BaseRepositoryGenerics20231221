using Newtonsoft.Json;
using WebAPI.Net6.BaseRepositoryGenerics.Domain.Models;
using WebAPI.Net6.BaseRepositoryGenerics.Infrastructure;

namespace WebAPI.Net6.BaseRepositoryGenerics.Application.ErrorMessage.Model.Response
{
    public partial class ErrorMessageQueryResponse : BaseResponse
    {
        /// <summary>
        /// Gets or sets the error messages.
        /// </summary>
        /// <value>
        /// The error messages.
        /// </value>
        [JsonProperty("errorMessage")]
        public List<TErrorMessage>? ErrorMessages { get; set; } = new List<TErrorMessage>();

        /// <summary>
        /// Gets or sets the index of the page.
        /// </summary>
        /// <value> The index of the page. </value>
        [JsonProperty("pageIndex")]
        public int? PageIndex { get; set; } = 1;

        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        /// <value> The size of the page. </value>
        [JsonProperty("pageSize")]
        public int? PageSize { get; set; } = 20;

        /// <summary>
        /// Gets or sets the total.
        /// </summary>
        /// <value> The total. </value>
        [JsonProperty("total")]
        public int? Total { get; set; } = 0;
    }
}
