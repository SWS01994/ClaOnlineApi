using System.Configuration;

namespace WebAPI.Common
{
    public static class AuthConfig
    {
        public static string SecretKey
        {
            get { return ConfigurationManager.AppSettings["Jwt.SecretKey"]; }
        }

        public static string Issuer
        {
            get { return ConfigurationManager.AppSettings["Jwt.Issuer"]; }
        }

        public static string Audience
        {
            get { return ConfigurationManager.AppSettings["Jwt.Audience"]; }
        }

        public static int ExpiryMinutes
        {
            get
            {
                string raw = ConfigurationManager.AppSettings["Jwt.ExpiryMinutes"];
                int minutes;
                if (!int.TryParse(raw, out minutes))
                {
                    minutes = 60;
                }
                return minutes;
            }
        }
    }
}