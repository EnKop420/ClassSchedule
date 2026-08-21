using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SchoolScheduleLibrary.Utilities.Auth
{
    /// <summary>
    /// Used for limiting endpoints to localhost only. This is for simulating endpoints that is only available for the product administrators.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class LocalhostOnlyAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Get the IP of the user that made the call
            var remoteIp = context.HttpContext.Connection.RemoteIpAddress;

            // Check if the ip is a localhost IP.
            if (remoteIp != null && (IPAddress.IsLoopback(remoteIp) || remoteIp.Equals(context.HttpContext.Connection.LocalIpAddress)))
            {
                await next();
                return;
            }

            context.Result = new NotFoundResult(); // Return 404 so potential attackers dont even know it exists
        }
    }
}
