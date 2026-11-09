using System.Web.Script.Services;
using System.Web.Services;
using WebAPI.BLL;
using WebAPI.Models.Requests;
using WebAPI.Models.Responses;

namespace WebAPI.WebService
{
    public partial class ClaOnlineApi
    {
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public ApiResponse<TokenResponse> GetToken(TokenRequest request)
        {
            return _authService.GetToken(request);
        }
    }
}