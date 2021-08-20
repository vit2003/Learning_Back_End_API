using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Security
{
    public class IsHostRequirement : IAuthorizationRequirement
    {
    }

    public class IsHostRequirementHandler : AuthorizationHandler<IsHostRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DataContext _dataContext;

        public IsHostRequirementHandler(IHttpContextAccessor httpContextAccessor, DataContext dataContext)
        {
            //here we use HttpContextAccessor, but not use IUserAccessor because the Infrastructure project like a fillter, it not good for touching something inside.
            _httpContextAccessor = httpContextAccessor;
            _dataContext = dataContext;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsHostRequirement requirement)
        {
            //get current user name
            var currentUsername = _httpContextAccessor.HttpContext.User?.Claims?.SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            //get activity
            //con context lấy request
            //trong request lấy value của key == id
            var activityId = Guid.Parse(_httpContextAccessor.HttpContext.Request.RouteValues.SingleOrDefault(x => x.Key == "id").Value.ToString());

            var activity = _dataContext.Activities.Include(x => x.UserActivities)
                .ThenInclude(x => x.AppUser)
                .FirstOrDefaultAsync(x => x.Id == activityId).Result;

            //get host of activity vừa get lên
            var host = activity.UserActivities.FirstOrDefault(x => x.IsHost);

            if(host?.AppUser?.UserName == currentUsername)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
