using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace EcoClean.MVC.Middleware;

/// <summary>
/// Reads the JWT from the ecoclean_token cookie, validates it,
/// and populates HttpContext.Items so controllers can read IsPremium / Role.
/// </summary>
public class JwtCookieMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _secret;
    private readonly string _issuer;
    private readonly string _audience;

    public JwtCookieMiddleware(RequestDelegate next, IConfiguration config)
    {
        _next     = next;
        _secret   = config["JwtSettings:Secret"]   ?? string.Empty;
        _issuer   = config["JwtSettings:Issuer"]   ?? string.Empty;
        _audience = config["JwtSettings:Audience"] ?? string.Empty;
    }

    public async Task InvokeAsync(HttpContext ctx)
    {
        var token = ctx.Request.Cookies["ecoclean_token"];

        // Cookie name/role tồn tại nhưng token không có → cookie Secure cũ bị stuck
        // Xóa hết và redirect login để user lấy cookie mới
        var hasStaleSession = ctx.Request.Cookies.ContainsKey("ecoclean_name")
                           && string.IsNullOrEmpty(token)
                           && ctx.Request.Path != "/Account/Login"
                           && ctx.Request.Path != "/Account/Register"
                           && ctx.Request.Path != "/Account/Logout";

        if (hasStaleSession)
        {
            ctx.Response.Cookies.Delete("ecoclean_name");
            ctx.Response.Cookies.Delete("ecoclean_role");
            ctx.Response.Redirect("/Account/Login");
            return;
        }

        if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(_secret))
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var key     = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));

                var principal = handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer           = true,
                    ValidateAudience         = true,
                    ValidateLifetime         = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer              = _issuer,
                    ValidAudience            = _audience,
                    IssuerSigningKey         = key,
                    ClockSkew                = TimeSpan.Zero
                }, out _);

                var isPremiumClaim = principal.FindFirst("IsPremium")?.Value;
                var roleClaim      = principal.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

                ctx.Items["IsPremium"] = isPremiumClaim == "True" ? "True" : "False";
                ctx.Items["Role"]      = roleClaim ?? "User";
            }
            catch
            {
                // Invalid / expired token — clear cookies
                ctx.Response.Cookies.Delete("ecoclean_token");
                ctx.Response.Cookies.Delete("ecoclean_name");
                ctx.Response.Cookies.Delete("ecoclean_role");
            }
        }

        await _next(ctx);
    }
}
