using System.ComponentModel.DataAnnotations;
using WebAPI.Net6.BaseRepositoryGenerics.Domain.Models;

namespace WebAPI.Net6.BaseRepositoryGenerics.Application.ErrorMessage.Model.Request
{
    public partial class ErrorMessageQueryRequest
    {
        /// <summary>
        /// 产品类型，多个用英文逗号分隔。
        /// </summary>
        public string ProductTypes { get; set; } = null!;
        /// <summary>
        /// 错误信息关键字
        /// </summary>
        public string Keywords { get; set; } = null!;

        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        /// <value> The size of the page. </value>
        [Range(1, 50, ErrorMessage = "Value must be between 1 and 50")]
        public int PageSize { get; set; } = 20;

        /// <summary>
        /// Gets or sets the index of the page.
        /// </summary>
        /// <value> The index of the page. </value>
        [Range(1, 1000, ErrorMessage = "Value must be between 1 and 1000")]
        public int PageIndex { get; set; } = 1;
    }
}
