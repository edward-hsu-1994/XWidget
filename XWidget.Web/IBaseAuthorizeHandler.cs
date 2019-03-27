using System.Threading.Tasks;

namespace XWidget.Web {
    public interface IBaseAuthorizeHandler {
        Task<bool> Authorize(string account, string password);
    }
}