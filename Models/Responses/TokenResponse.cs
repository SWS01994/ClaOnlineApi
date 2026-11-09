using System;

namespace WebAPI.Models.Responses
{
    public class TokenResponse
    {
        public string Token { get; set; }
        public DateTime ExpiresAtUtc { get; set; }
    }
}