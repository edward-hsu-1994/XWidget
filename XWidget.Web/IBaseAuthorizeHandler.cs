using System.Threading.Tasks;

namespace XWidget.Web {
    public interface IBaseAuthorizeHandler {
        /// <summary>
        /// 驗證方法
        /// </summary>
        Task<bool> Authorize(string account, string password);
    }
}