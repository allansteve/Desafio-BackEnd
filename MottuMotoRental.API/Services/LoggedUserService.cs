using MottuMotoRental.Core.Interfaces;
using MottuMotoRental.Infrastructure.Data.Util;
using System.Security.Claims;

namespace MottuMotoRental.API.Services
{
    
    public class LoggedUserService:ILoggedUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public LoggedUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor; 
        }

        public Guid? UserId => CurrentUserUtil.GetUserId(_httpContextAccessor.HttpContext);        

        public IEnumerable<string>? Roles => CurrentUserUtil.GetRoles(_httpContextAccessor.HttpContext);
    }

    public static class CurrentUserUtil
    {
        public static Guid? GetUserId(HttpContext? httpContext)
        {
            var userId = httpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return null;

            return new Guid(userId);
        }

        public static IEnumerable<string>? GetRoles(HttpContext? httpContext) =>
            httpContext?.User.FindAll(ClaimTypes.Role).Select(x => x.Value);
    }

}

