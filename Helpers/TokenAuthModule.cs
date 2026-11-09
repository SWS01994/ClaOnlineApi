using System;
using System.Text;
using System.Web;

namespace WebAPI.Helpers
{
    public class TokenAuthModule : IHttpModule
    {
        // Method paths that don't require a token.
        private static readonly string[] AnonymousPaths = new string[]
        {
            "/webservice/claonlineapi.asmx/gettoken"
        };

        public void Init(HttpApplication context)
        {
            context.BeginRequest += Context_BeginRequest;
        }

        private void Context_BeginRequest(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;
            HttpRequest request = app.Context.Request;

            string path = request.Path.ToLowerInvariant();

            // Only guard our ASMX service. Everything else on the site is untouched.
            if (path.IndexOf("/webservice/claonlineapi.asmx", StringComparison.Ordinal) < 0)
            {
                return;
            }

            // Let the ASMX help page / WSDL (GET requests) through.
            if (request.HttpMethod == "GET")
            {
                return;
            }

            // Let the token-issuing method itself through.
            if (IsAnonymousPath(path))
            {
                return;
            }

            string authHeader = request.Headers["Authorization"];
            string token = ExtractBearerToken(authHeader);

            string clientId;
            if (token == null || !JwtHelper.ValidateToken(token, out clientId))
            {
                WriteUnauthorized(app.Context);
                app.CompleteRequest();
            }
        }

        private bool IsAnonymousPath(string path)
        {
            for (int i = 0; i < AnonymousPaths.Length; i++)
            {
                if (path.IndexOf(AnonymousPaths[i], StringComparison.Ordinal) >= 0)
                {
                    return true;
                }
            }
            return false;
        }

        private string ExtractBearerToken(string authHeader)
        {
            if (string.IsNullOrEmpty(authHeader))
            {
                return null;
            }

            string prefix = "Bearer ";
            if (authHeader.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                return authHeader.Substring(prefix.Length).Trim();
            }

            return null;
        }

        private void WriteUnauthorized(HttpContext context)
        {
            context.Response.Clear();
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";

            StringBuilder json = new StringBuilder();
            json.Append("{\"Status\":\"Error\",\"StatusCode\":401,");
            json.Append("\"Message\":\"Unauthorized. A valid Bearer token is required.\",");
            json.Append("\"Data\":null,\"Errors\":null}");

            context.Response.Write(json.ToString());
        }

        public void Dispose()
        {
        }
    }
}