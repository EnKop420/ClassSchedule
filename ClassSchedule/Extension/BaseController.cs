using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolScheduleLibrary.Enums;
using SchoolScheduleLibrary.Utilities.Auth;

namespace ClassSchedule.Inheritance
{
    /// <summary>
    /// Made to save specific data from the session when authenticated. Gets wiped after the request is finished.
    /// This class is inherited by the controllers.
    /// </summary>
    public class BaseController : ControllerBase
    {
        protected SessionData Session =>
            (SessionData)HttpContext.Items["SessionData"]!;

        protected Guid CurrentInstitutionId => Guid.Parse(Session.InstitutionId);
        protected Guid CurrentUserId => Guid.Parse(Session.UserId);
        protected UserRoles CurrentUserRole => Session.Role;
    }
}
