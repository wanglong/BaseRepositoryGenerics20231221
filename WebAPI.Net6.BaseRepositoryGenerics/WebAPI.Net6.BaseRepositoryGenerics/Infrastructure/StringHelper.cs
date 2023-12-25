using System.Text;

namespace WebAPI.Net6.BaseRepositoryGenerics.Infrastructure
{
    /// <summary>
    /// String Helper.
    /// </summary>
    public static class StringHelper
    {
        public static string BuildExceptionMessage(System.Exception ex)
        {
            StringBuilder exceptionMsgs = new StringBuilder();
            if (ex != null)
            {
                exceptionMsgs.AppendFormat("\n{0}\n{1}\n{2} Soure:{3}", ex.GetType().Name, ex.Message, ex.StackTrace, ex.Source);

                // lookup inner exception.
                if (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    exceptionMsgs.AppendFormat("\n{0}\n{1}\n{2} Soure:{3}", ex.GetType().Name, ex.Message, ex.StackTrace, ex.Source);
                }
            }

            return exceptionMsgs.ToString();

        }
    }
}
