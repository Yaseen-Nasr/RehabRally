using RehabRally.Web.Core.AuthModels;

namespace RehabRally.Web.Services
{
    public interface IAuthService
    {
         Task<AuthModel> GetTokenAsync(TokenRequestModel model); 
    }
}
