using Application.Extensions;
using Domain;
using MediatR;
using Persistence;

namespace Application.Activities.Commands;

public class EditActivity
{
    public record Command(Activity Activity) : IRequest;

    public class Handler(AppDbContext context) : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var activity = await context.Activities
                .FindAsync([request.Activity.Id], cancellationToken)
                    ?? throw new Exception("Cannot find actiivity");

            ActivityMapper.ToActivityDao(request.Activity, activity);

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
