using MediatR;
using Persistence;

namespace Application.Activities.Commands;

public class DeleteActivity
{
    public record Command(string Id) : IRequest;

    public class Handler(AppDbContext context) : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var activity = await context.Activities
                .FindAsync([request.Id], cancellationToken)
                    ?? throw new Exception("Cannot find actiivity");

            context.Activities.Remove(activity);

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
