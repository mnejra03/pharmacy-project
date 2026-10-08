namespace NewPharmacy.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class MyAuthorizationAttribute(bool isAdmin, bool isPharmacist, bool isCustomer) : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var authService = context.HttpContext.RequestServices.GetService<MyAuthService>();
        if (authService == null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var authInfo = authService.GetAuthInfo();
        if (authInfo == null || !authInfo.IsLoggedIn)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var requiresSpecificRole = isAdmin || isPharmacist || isCustomer;
        if (!requiresSpecificRole)
        {
            return;
        }

        var isAuthorized =
            (isAdmin && authInfo.IsAdmin) ||
            (isPharmacist && authInfo.IsPharmacist) ||
            (isCustomer && authInfo.IsCustomer);

        if (!isAuthorized)
        {
            context.Result = new ForbidResult();
        }
    }
}
