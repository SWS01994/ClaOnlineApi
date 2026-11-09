using System;
using System.Collections.Generic;
using System.Configuration;
using WebAPI.Common;
using WebAPI.Helpers;
using WebAPI.Models.Requests;
using WebAPI.Models.Responses;

namespace WebAPI.BLL
{
    public class AuthService
    {
        public ApiResponse<TokenResponse> GetToken(TokenRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.ClientId) || string.IsNullOrEmpty(request.ClientSecret))
                {
                    List<string> errs = new List<string>();
                    errs.Add("ClientId and ClientSecret are required.");
                    return ApiResponseFactory.ValidationError<TokenResponse>(errs);
                }

                if (!IsValidClient(request.ClientId, request.ClientSecret))
                {
                    return ApiResponseFactory.Error<TokenResponse>("Invalid client credentials.");
                }

                string token = JwtHelper.GenerateToken(request.ClientId);

                TokenResponse resp = new TokenResponse();
                resp.Token = token;
                resp.ExpiresAtUtc = DateTime.UtcNow.AddMinutes(AuthConfig.ExpiryMinutes);

                return ApiResponseFactory.Success<TokenResponse>(resp, "Token issued successfully.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "AuthService.GetToken");
                return ApiResponseFactory.Error<TokenResponse>(Messages.ServerError);
                // TEMPORARY — reveals the real error. Revert after debugging.
                //string detail = "EX: " + ex.GetType().FullName + " | MSG: " + ex.Message;
                //if (ex.InnerException != null)
                //{
                //    detail += " | INNER: " + ex.InnerException.GetType().FullName + ": " + ex.InnerException.Message;
                //}
                //return ApiResponseFactory.Error<TokenResponse>(detail);
            }
        }

        private bool IsValidClient(string clientId, string clientSecret)
        {
            string configId = ConfigurationManager.AppSettings["Auth.ClientId"];
            string configSecret = ConfigurationManager.AppSettings["Auth.ClientSecret"];
            return clientId == configId && clientSecret == configSecret;
        }
    }
}