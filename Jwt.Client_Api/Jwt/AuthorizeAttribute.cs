using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Diagnostics;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public string UserType { get; set; } //Used like this: [Authorize (UserType="worker")]


    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var ut = context.HttpContext.Items["user_type"]?.ToString().ToLower();

        var UnAuthorized = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };

        string msg = "";

        if (ut == null)
        {
            msg = $"UNAUTHORIZED  -  'UserType' NOT FOUND";
            context.Result = UnAuthorized;
        }
        else if (ut != UserType)
        {
            msg = $"UNAUTHORIZED  -  'UserType' NOT FOUND.  UserType actually passed in: {ut}";
            context.Result = UnAuthorized;
        }
        else
        {
            // Authorized :-)
            msg = $"AUTHORIZED  -  {ut}";
        }
        //Log.Logger.Debug(msg);
        Debug.WriteLine(msg);
    }
}