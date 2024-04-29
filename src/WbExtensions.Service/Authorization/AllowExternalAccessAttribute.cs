using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WbExtensions.Service.Authorization;


[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
internal sealed class AllowExternalAccessAttribute : Attribute, IAuthorizationFilter
{
    private readonly bool _allowExternalAccess;

    public AllowExternalAccessAttribute(bool allowExternalAccess)
    {
        _allowExternalAccess = allowExternalAccess;
    }
    
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var externalHeaderExists =
            context.HttpContext.Request.Headers.TryGetValue(AuthConstants.ExternalHeaderName, out var externalHeader);

        if (_allowExternalAccess)
        {
            var configuration = context.HttpContext.RequestServices.GetService<IConfiguration>()!;
            var externalHeaderValid = configuration["ExternalHeaderValue"] == externalHeader;
            if (externalHeaderExists && !externalHeaderValid)
            {
                context.Result = new UnauthorizedResult();
            }
        }
        else
        {
            if (externalHeaderExists)
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}