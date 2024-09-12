using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Response;
using warehouse_BE.Application.Storages.Queries.GetStorageList;
using warehouse_BE.Domain.Entities;

namespace warehouse_BE.Application.Storages.Commads.CreateStorage;

public record CreateStorageCommand : IRequest<ResponseDto>
{
    public required string Name { get; init; }
    public required string Location { get; init; }
    public required string Status { get; init; }
    public ICollection<Area>? Areas { get; init; }
}

public class CreateStorageCommandHandler : IRequestHandler<CreateStorageCommand, ResponseDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _currentUser;
    private readonly IMapper _mapper;

    public CreateStorageCommandHandler(IApplicationDbContext context, IUser currentUser, IMapper mapper)
    {
        _context = context;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<ResponseDto> Handle(CreateStorageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = new Storage
            {
                Name = request.Name,
                Location = request.Location,
                Status = request.Status,
                Areas = _mapper.Map<ICollection<Area>>(request.Areas), // Map AreaDto to Area
                Created = DateTimeOffset.UtcNow,
                CreatedBy = _currentUser.Id,
                LastModified = DateTimeOffset.UtcNow,
                LastModifiedBy = _currentUser.Id
            };

            _context.Storage.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            var createdStorageDto = _mapper.Map<StorageDto>(entity); // Map Storage to StorageDto

            return new ResponseDto(201, "Storage created successfully", createdStorageDto);
        }
        catch (Exception ex)
        {
            return new ResponseDto(500, $"An error occurred while creating the storage: {ex.Message}");
        }
    }
}
