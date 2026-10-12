using Domain;
using MediatR;
using Persistence;

namespace Application.Activities.Queries;

public class GetActivityDetails
{
    public record Query(string Id) : IRequest<Activity>;

    public class Handler(AppDbContext context) : IRequestHandler<Query, Activity>
    {
        public async Task<Activity> Handle(Query request, CancellationToken cancellationToken)
        {
            return await context.Activities.FindAsync([request.Id], cancellationToken) ??
                throw new Exception("activity not found");

        }
    }
}