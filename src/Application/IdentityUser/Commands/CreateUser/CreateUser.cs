
using warehouse_BE.Application.Common.Interfaces;

namespace warehouse_BE.Application.IdentityUser.Commands.CreateUser;

public record CreateUserCommand : IRequest<int>
{
    public string? Email { get; init; }

    public string? Password { get; init; }
}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    public CreateUserCommandHandler(IApplicationDbContext context, IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async Task<int> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return 0; 
        }

        var (result, userId) = await _identityService.CreateUserAsync(request.Email, request.Password);

        return result.Succeeded ? 1 : 0;
    }
}