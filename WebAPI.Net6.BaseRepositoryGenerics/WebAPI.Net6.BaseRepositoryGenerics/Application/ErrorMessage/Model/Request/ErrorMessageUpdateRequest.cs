namespace WebAPI.Net6.BaseRepositoryGenerics.Application.ErrorMessage.Model.Request
{
    public class ErrorMessageUpdateRequest
    {
        /// <summary>
        /// 自增id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 产品类型，多个用英文逗号分隔。
        /// </summary>
        public string ProductTypes { get; set; } = null!;
        /// <summary>
        /// 错误信息关键字
        /// </summary>
        public string Keywords { get; set; } = null!;
        /// <summary>
        /// 中文输出
        /// </summary>
        public string Cnoutput { get; set; } = null!;
        /// <summary>
        /// 英文输出
        /// </summary>
        public string Enoutput { get; set; } = null!;
        /// <summary>
        /// 原始错误信息
        /// </summary>
        public string OriginalMessage { get; set; } = null!;
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// 操作人
        /// </summary>
        public string Operator { get; set; } = null!;
        /// <summary>
        /// 是否启用(0:禁用, 1:启用)
        /// </summary>
        public bool IsEnabled { get; set; }
    }
}
