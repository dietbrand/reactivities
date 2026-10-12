using Domain;
using MediatR;
using Persistence;

namespace Application.Activities.Commands;

public class CreateActivity
{
    public record Command(Activity Activity) : IRequest<string>;

    public class Handler(AppDbContext context) : IRequestHandler<Command, string>
    {
        public async Task<string> Handle(Command request, CancellationToken cancellationToken)
        {
            context.Activities.Add(request.Activity);

            await context.SaveChangesAsync(cancellationToken);

            return request.Activity.Id;
        }
    }
}
