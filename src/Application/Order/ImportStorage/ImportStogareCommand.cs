using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Response;

namespace warehouse_BE.Application.Order.ImportStorage;

public class ImportStogareCommand : IRequest<ResponseDto>
{
    public required string Type { get; set; }
    public decimal TotalPrice { get; set; }
    public List<ImportProductDto>? Products { get; set; }
}
public class ImportStogareCommandHandler : IRequestHandler<ImportStogareCommand, ResponseDto> 
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly IUser currentUser;

    public ImportStogareCommandHandler(IApplicationDbContext context, IIdentityService identityService, IUser currentUser)
    {
        this._context = context;
        this._identityService = identityService;
        this.currentUser = currentUser;
    }
    public async Task<ResponseDto> Handle (ImportStogareCommand request, CancellationToken cancellationToken)
    {
        var rs = new ResponseDto();
        if (currentUser.Id != null)
        {
            var userId = await _identityService.GetUserByIdAsync(currentUser.Id);
        }
        return rs;
    }
}
