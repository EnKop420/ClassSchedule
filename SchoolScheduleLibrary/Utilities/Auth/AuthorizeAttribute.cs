using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using SchoolScheduleLibrary.Repository.Interface;
using SchoolScheduleLibrary.Utilities.Auth;
using SchoolScheduleLibrary.Utilities.Response;

namespace ClassSchedule.Auth
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class AuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public string Role { get; set; }
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            string sessionKey = "";
            context.HttpContext.Request.Cookies.TryGetValue("schoolsession", out string? accessTokenFromCookie);
            if (!string.IsNullOrEmpty(accessTokenFromCookie))
            {
                try
                {
                    IRedisRepository redisRepository = context.HttpContext.RequestServices.GetRequiredService<IRedisRepository>();

                    sessionKey = accessTokenFromCookie;
                    SessionData sessionData = await redisRepository.GetSessionDataFromKey(sessionKey);
                    if (sessionData.Role == Role)
                    {
                        return;
                    }
                }
                catch (HttpResponseException hre)
                {
                    context.Result = new ObjectResult(new { message = hre.ResponseMessage })
                    {
                        StatusCode = (int)hre.StatusCode
                    };
                    return;
                }
            }
            context.Result = new UnauthorizedResult();
            return;
        }
    }
}