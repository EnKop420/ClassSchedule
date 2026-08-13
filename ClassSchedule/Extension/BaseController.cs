using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolScheduleLibrary.Enums;
using SchoolScheduleLibrary.Utilities.Auth;

namespace ClassSchedule.Inheritance
{
    public class BaseController : ControllerBase
    {
        protected SessionData Session =>
            (SessionData)HttpContext.Items["SessionData"]!;

        protected Guid CurrentInstitutionId => Guid.Parse(Session.InstitutionId);
        protected Guid CurrentUserId => Guid.Parse(Session.UserId);
        protected UserRoles CurrentUserRole => Session.Role;
    }
}
