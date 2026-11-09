using System;
using System.Security.Cryptography;
using System.Text;
using WebAPI.Common;

namespace WebAPI.Helpers
{
    public static class JwtHelper
    {
        public static string GenerateToken(string clientId)
        {
            long nowUnix = ToUnixTime(DateTime.UtcNow);
            long expUnix = ToUnixTime(DateTime.UtcNow.AddMinutes(AuthConfig.ExpiryMinutes));

            string headerJson = "{\"alg\":\"HS256\",\"typ\":\"JWT\"}";

            string payloadJson = "{"
                + "\"sub\":\"" + EscapeJson(clientId) + "\","
                + "\"iss\":\"" + EscapeJson(AuthConfig.Issuer) + "\","
                + "\"aud\":\"" + EscapeJson(AuthConfig.Audience) + "\","
                + "\"iat\":" + nowUnix + ","
                + "\"nbf\":" + nowUnix + ","
                + "\"exp\":" + expUnix
                + "}";

            string headerEncoded = Base64UrlEncode(Encoding.UTF8.GetBytes(headerJson));
            string payloadEncoded = Base64UrlEncode(Encoding.UTF8.GetBytes(payloadJson));

            string unsignedToken = headerEncoded + "." + payloadEncoded;
            string signature = Sign(unsignedToken);

            return unsignedToken + "." + signature;
        }

        public static bool ValidateToken(string token, out string clientId)
        {
            clientId = null;

            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            string[] parts = token.Split('.');
            if (parts.Length != 3)
            {
                return false;
            }

            string headerEncoded = parts[0];
            string payloadEncoded = parts[1];
            string signature = parts[2];

            // 1. Verify signature
            string unsignedToken = headerEncoded + "." + payloadEncoded;
            string expectedSignature = Sign(unsignedToken);

            if (!SlowEquals(signature, expectedSignature))
            {
                return false;
            }

            // 2. Decode and check payload (expiry, issuer, audience)
            try
            {
                string payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(payloadEncoded));

                string sub = ExtractJsonValue(payloadJson, "sub");
                string iss = ExtractJsonValue(payloadJson, "iss");
                string aud = ExtractJsonValue(payloadJson, "aud");
                string expStr = ExtractJsonValue(payloadJson, "exp");

                if (iss != AuthConfig.Issuer || aud != AuthConfig.Audience)
                {
                    return false;
                }

                long expUnix;
                if (!long.TryParse(expStr, out expUnix))
                {
                    return false;
                }

                DateTime expiry = FromUnixTime(expUnix);
                if (DateTime.UtcNow > expiry.AddSeconds(30)) // 30s clock skew allowance
                {
                    return false;
                }

                clientId = sub;
                return true;
            }
            catch
            {
                return false;
            }
        }

        // ================= internals =================

        private static string Sign(string data)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(AuthConfig.SecretKey);
            using (HMACSHA256 hmac = new HMACSHA256(keyBytes))
            {
                byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return Base64UrlEncode(hash);
            }
        }

        private static bool SlowEquals(string a, string b)
        {
            // Constant-time comparison to avoid timing attacks.
            if (a == null || b == null || a.Length != b.Length)
            {
                return false;
            }

            int diff = 0;
            for (int i = 0; i < a.Length; i++)
            {
                diff |= a[i] ^ b[i];
            }
            return diff == 0;
        }

        private static string Base64UrlEncode(byte[] bytes)
        {
            string s = Convert.ToBase64String(bytes);
            s = s.Replace('+', '-').Replace('/', '_');
            while (s.EndsWith("=", StringComparison.Ordinal))
            {
                s = s.Substring(0, s.Length - 1);
            }
            return s;
        }

        private static byte[] Base64UrlDecode(string s)
        {
            string padded = s.Replace('-', '+').Replace('_', '/');
            switch (padded.Length % 4)
            {
                case 2: padded += "=="; break;
                case 3: padded += "="; break;
            }
            return Convert.FromBase64String(padded);
        }

        private static long ToUnixTime(DateTime dt)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(dt - epoch).TotalSeconds;
        }

        private static DateTime FromUnixTime(long unixSeconds)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixSeconds);
        }

        private static string EscapeJson(string s)
        {
            if (s == null) return "";
            return s.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        // Minimal string-value extractor for our own flat, known JSON shape.
        // Safe here because we generated this JSON ourselves (no untrusted structure).
        private static string ExtractJsonValue(string json, string key)
        {
            string quotedKey = "\"" + key + "\":";
            int idx = json.IndexOf(quotedKey, StringComparison.Ordinal);
            if (idx < 0) return null;

            int start = idx + quotedKey.Length;

            if (start < json.Length && json[start] == '\"')
            {
                // string value
                start++;
                int end = json.IndexOf('\"', start);
                return json.Substring(start, end - start);
            }
            else
            {
                // numeric value
                int end = start;
                while (end < json.Length && (char.IsDigit(json[end]) || json[end] == '-'))
                {
                    end++;
                }
                return json.Substring(start, end - start);
            }
        }
    }
}