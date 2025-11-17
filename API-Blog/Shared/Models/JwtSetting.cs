namespace Shared.Models
{
    public class JwtSetting
    {
        public string SecretKey { get; set; } = "";
        public string ValidAudience { get; set; } = "";
        public string ValidIssuer { get; set; } = "";
        public int TokenValidityInMinutes { get; set; } = 500;
        public int RefreshTokenValidityInDays { get; set; } = 7;
    }
}
