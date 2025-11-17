using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Shared.ClaimService
{
    public class ClaimService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public ClaimService(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
        private HttpContext Context => _contextAccessor.HttpContext;

        public string? UserName
        {
            get
            {
                var userName = "";
                var u = Context?.User;

                if (u != null)
                {
                    userName = u.FindFirst(JwtRegisteredClaimNames.Name)?.Value;
                }

                return userName;
            }
        }

        public Guid UserId
        {
            get
            {
                var rs = Guid.Empty;
                var u = Context?.User;
                if (u != null)
                {
                    try
                    {
                        rs = Guid.Parse(u.FindFirst(JwtRegisteredClaimNames.Jti)?.Value);
                    }
                    catch
                    {
                    }
                }

                return rs;
            }
        }

        public string Email
        {
            get
            {
                var rs = "";
                var u = Context?.User;
                if (u != null)
                {
                    try
                    {
                        rs = u.FindFirst(c => c.Type == ClaimTypes.Email)?.Value;
                    }
                    catch
                    {
                    }
                }

                return rs;
            }
        }

        public string Locale
        {
            get
            {
                string str = "ja";
                try
                {
                    var locale = _contextAccessor.HttpContext.Request.Headers["Accept-Language"].ToString();
                    if (!string.IsNullOrEmpty(locale))
                    {
                        var arr = locale.Split(",");
                        str = arr[0];
                    }
                }
                catch
                {
                }

                return str;
            }
        }

        public List<string> Roles
        {
            get
            {
                List<string> listRole = new();
                var u = _contextAccessor.HttpContext?.User;

                if (u != null)
                {
                    listRole = u.Claims
                        .Where(c => c.Type == ClaimTypes.Role)
                        .Select(c => c.Value)
                        .ToList();
                }

                return listRole;
            }
        }

        public bool IsAdmin
        {
            get
            {
                try
                {
                    List<string> listRole = new();
                    var u = _contextAccessor.HttpContext?.User;

                    if (u != null)
                    {
                        listRole = u.Claims
                            .Where(c => c.Type == ClaimTypes.Role)
                            .Select(c => c.Value)
                            .ToList();
                    }

                    if (listRole.Any())
                        return listRole.Any(x => x.Trim().ToLower() == "admin");
                    return false;
                }
                catch
                {
                    return false;
                }
            }
        }

        public bool IsUser
        {
            get
            {
                try
                {
                    List<string> listRole = new();
                    var u = _contextAccessor.HttpContext?.User;

                    if (u != null)
                    {
                        listRole = u.Claims
                            .Where(c => c.Type == ClaimTypes.Role)
                            .Select(c => c.Value)
                            .ToList();
                    }

                    if (listRole.Any())
                        return listRole.Any(x => x.Trim() == "User");
                    return false;
                }
                catch
                {
                    return false;
                }
            }
        }

        public bool IsCustomer
        {
            get
            {
                try
                {
                    List<string> listRole = new();
                    var u = _contextAccessor.HttpContext?.User;

                    if (u != null)
                    {
                        listRole = u.Claims
                            .Where(c => c.Type == ClaimTypes.Role)
                            .Select(c => c.Value)
                            .ToList();
                    }

                    if (listRole.Any())
                        return listRole.Any(x => x.Trim() == "Customer");
                    else return false;
                }
                catch
                {
                    return false;
                }
            }
        }

        public string AccessToken
        {
            get
            {
                string str = "";
                try
                {
                    str = _contextAccessor.HttpContext.GetTokenAsync("access_token").Result;
                }
                catch
                {
                }

                return str;
            }
        }
    }
}
