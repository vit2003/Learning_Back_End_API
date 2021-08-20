using Application.Errors;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistance;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Activities
{
    public class Details
    {
        public class Query : IRequest<ActivityDto>
        {
            public Guid Id { get; set; }
        }
        public class Handler : IRequestHandler<Query, ActivityDto>
        {
            private readonly DataContext _context;
            private readonly IMapper mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                this.mapper = mapper;
                this._context = context;
            }

            public async Task<ActivityDto> Handle(Query request, CancellationToken cancellationToken)
            {
                var activities = await _context.Activities
                .Include(x => x.UserActivities)
                .ThenInclude(x => x.AppUser)
                .FirstOrDefaultAsync(x => x.Id == request.Id);

                if (activities == null)
                {
                    throw new RestException(System.Net.HttpStatusCode.NotFound, new { activity = "NOT FOUND" });
                }

                var activityToReturn = mapper.Map<Activity, ActivityDto>(activities);

                return activityToReturn;
            }
        }
    }
}
