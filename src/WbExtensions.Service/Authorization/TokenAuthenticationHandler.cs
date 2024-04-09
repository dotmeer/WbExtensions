using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WbExtensions.Application.Interfaces.Yandex;

namespace WbExtensions.Service.Authorization;

internal sealed class TokenAuthenticationHandler : AuthenticationHandler<TokenAuthenticationOptions>
{
    private readonly IUserService _userService;

    public TokenAuthenticationHandler(IOptionsMonitor<TokenAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, IUserService userService)
        : base(options, logger, encoder)
    {
        _userService = userService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            var token = authHeader.ToString().Replace("Bearer ", "");
            var userId = await _userService.GetUserIdAsync(token, Context.RequestAborted);

            if (userId is not null)
            {
                var claims = new[] { new Claim(AuthConstants.UserIdClaim, userId) };
                var identity = new ClaimsIdentity(claims, nameof(TokenAuthenticationHandler));
                var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), this.Scheme.Name);
                return AuthenticateResult.Success(ticket);
            }
        }
        
        return AuthenticateResult.Fail("No auth");
    }
}