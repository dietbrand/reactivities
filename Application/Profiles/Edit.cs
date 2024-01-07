using Application.Core;
using Application.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles
{
  public class Edit
  {
    public class Command : IRequest<Result<Unit>>
    {
      public Profile Profile { get; set; }
    }
    public class CommandValidator : AbstractValidator<Command>
    {
      public CommandValidator()
      {
        RuleFor(x => x.Profile).SetValidator(new ProfileValidator());
      }
    }
    public class Handler : IRequestHandler<Command, Result<Unit>>
    {
      private readonly DataContext _context;
      private readonly IUserAccessor _userAccessor;
      public Handler(DataContext context, IUserAccessor userAccessor)
      {
        _userAccessor = userAccessor;
        _context = context;
      }

      public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
      {
        var profile = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
        if (profile == null) return null;

        profile.DisplayName = request.Profile.DisplayName;
        profile.Bio = request.Profile.Bio;

        var result = await _context.SaveChangesAsync() > 0;
        if (!result) return Result<Unit>.Failure("Failed to edit the profile");
        return Result<Unit>.Success(Unit.Value);
      }
    }
  }
}