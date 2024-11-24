using System.Threading;
using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Response;

namespace warehouse_BE.Application.Storages.Commads.DeleteStorage;

public class DeleteStorageCommand : IRequest<ResponseDto>
{
    public required int Id { get; set; }
}
public class DeleteStorageCommandHandler : IRequestHandler<DeleteStorageCommand, ResponseDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;
    private readonly IIdentityService _identityService;
    public DeleteStorageCommandHandler(IApplicationDbContext context, IUser user, IIdentityService identityService)
    {
        _context = context;
        _user = user;
        _identityService = identityService;
    }
    public async Task<ResponseDto> Handle(DeleteStorageCommand request, CancellationToken CancellationToken)
    {
        if(_user.Id != null)
        {
            var storages = await _identityService.GetUserStoragesAsync(_user.Id);
            var storagesId = storages.Select(x => x.Id).ToList();
            // check request id in list 
            if (!storagesId.Contains(request.Id))
            {
                return new ResponseDto
                {
                    StatusCode   = 400,
                    Message = "Storage not found or access denied."
                };
            }

            // update delete 
            var storage = await _context.Storages.Where(o => o.Id == request.Id).FirstOrDefaultAsync();
            if (storage != null)
            {
                storage.IsDeleted = true;

                await _context.SaveChangesAsync(CancellationToken);

                return new ResponseDto
                {
                    StatusCode = 200,
                    Message = "Storage deleted successfully."
                };
            }
          
        }
        return new ResponseDto();
    }
}
