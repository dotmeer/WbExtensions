using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using WbExtensions.Application.Interfaces.Yandex;

namespace WbExtensions.Service.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
internal sealed class YandexAuthorizationAttribute : Attribute, IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var userService = context.HttpContext.RequestServices.GetService<IUserService>()!;

        if (context.HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            var token = authHeader.ToString().Replace("Bearer ", "");
            var userId = await userService.GetUserIdAsync(token, context.HttpContext.RequestAborted);

            if (userId is not null)
            {
                var claims = new[] { new Claim(AuthConstants.UserIdClaim, userId) };
                var identity = new ClaimsIdentity(claims, AuthConstants.AuthenticationType);
                context.HttpContext.User.AddIdentity(identity);
                return;
            }
        }

        context.Result = new UnauthorizedResult();
    }
}