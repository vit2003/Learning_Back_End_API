using Application.Errors;
using Domain;
using MediatR;
using Persistance;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Activities
{
    public class Details
    {
        public class Query : IRequest<Activity>
        {
            public Guid Id { get; set; }
        }
        public class Handler : IRequestHandler<Query, Activity>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                this._context = context;
            }

            public async Task<Activity> Handle(Query request, CancellationToken cancellationToken)
            {
                var activities = await _context.Activities.FindAsync(request.Id);

                if (activities == null)
                {
                    throw new RestException(System.Net.HttpStatusCode.NotFound, new { activity = "NOT FOUND" });
                }


                return activities;
            }
        }
    }
}
