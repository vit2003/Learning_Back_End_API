using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Persistance;

namespace Application.User
{
    public class CurrentUser
    {
        public class Query : IRequest<User>
        {

        }
        //get access to db context
        public class Handler : IRequestHandler<Query, User>
        {
            private readonly UserManager<AppUser> userManager;
            private readonly IJWTGenerator jwtGenerator;
            private readonly IUserAccessor userAccessor;

            public Handler(UserManager<AppUser> userManager, IJWTGenerator jwtGenerator, IUserAccessor userAccessor)
            {
                this.userAccessor = userAccessor;
                this.jwtGenerator = jwtGenerator;
                this.userManager = userManager;

            }
            public async Task<User> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await userManager.FindByNameAsync(userAccessor.GetCurrentUsername());

                return new User
                {
                    DisplayName = user.UserName,
                    Username = user.UserName,
                    Token = jwtGenerator.CreateToken(user),
                    Image = null
                };
            }
        }
    }
}