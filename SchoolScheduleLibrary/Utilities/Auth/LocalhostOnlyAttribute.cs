using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SchoolScheduleLibrary.Utilities.Auth
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class LocalhostOnlyAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var remoteIp = context.HttpContext.Connection.RemoteIpAddress;

            if (remoteIp != null && (IPAddress.IsLoopback(remoteIp) || remoteIp.Equals(context.HttpContext.Connection.LocalIpAddress)))
            {
                await next();
                return;
            }

            context.Result = new NotFoundResult(); // Return 404 so potential attackers dont even know it exists
        }
    }
}
