using Microsoft.AspNetCore.Http;
using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Response;
using warehouse_BE.Domain.Entities;
using warehouse_BE.Domain.Enums;

namespace warehouse_BE.Application.Storages.Commads.UpdateStorage;

public class UpdateStorageCommand : IRequest<ResponseDto>
{
    public required int StorageId { get; set; }
    public string? Name { get; set; }
    public string? Location { get; set; }
    public string? Status { get; set; } 
    public List<AreaDto>? Areas { get; set; }

}
public class UpdateStorageCommandHandler : IRequestHandler<UpdateStorageCommand, ResponseDto>
{
    private readonly IApplicationDbContext _context;

    public UpdateStorageCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ResponseDto> Handle(UpdateStorageCommand request, CancellationToken cancellationToken)
    {
        var response = new ResponseDto
        {
            Message = "Failed to update storage: an unexpected error occurred.",
            StatusCode = StatusCodes.Status400BadRequest
        };

        // Lấy Storage hiện tại
        var storage = await _context.Storages
            .Include(s => s.Areas)
            .FirstOrDefaultAsync(s => s.Id == request.StorageId, cancellationToken);

        if (storage == null)
        {
            response.Message = "Storage not found.";
            return response;
        }

        // Cập nhật các thuộc tính cơ bản
        if (!string.IsNullOrEmpty(request.Name))
        {
            storage.Name = request.Name;
        }

        if (!string.IsNullOrEmpty(request.Location))
        {
            storage.Location = request.Location;
        }

        if (!string.IsNullOrEmpty(request.Status))
        {
            if (Enum.TryParse(typeof(StorageStatus), request.Status, true, out var parsedStatus))
            {
                storage.Status = (StorageStatus)parsedStatus!;
            }
            else
            {
                response.Message = $"Invalid status value: {request.Status}.";
                return response;
            }
        }

        // Xử lý danh sách Areas
        if (request.Areas != null && request.Areas.Any())
        {
            foreach (var areaDto in request.Areas)
            {
                // Tìm Area trong danh sách hiện tại
                var existingArea = storage.Areas?.FirstOrDefault(a => a.Id == areaDto.Id);

                if (existingArea != null)
                {
                    // Cập nhật Area đã tồn tại
                    existingArea.Name = areaDto.Name;
                }
                else
                {
                    // Tạo mới Area và thêm vào danh sách
                    var newArea = new Area
                    {
                        Name = areaDto.Name
                    };
                    storage.Areas?.Add(newArea);
                }
            }
        }

        try
        {
            // Lưu các thay đổi
            _context.Storages.Update(storage);
            await _context.SaveChangesAsync(cancellationToken);

            response.Message = "Storage updated successfully.";
            response.StatusCode = StatusCodes.Status200OK;
            response.Data = new
            {
                storage.Id,
                storage.Name,
                storage.Location,
                Status = storage.Status.ToString(),
                storage.CompanyId,
                Areas = storage.Areas?.Select(a => new
                {
                    a.Id,
                    a.Name
                }).ToList()
            };
        }
        catch (Exception ex)
        {
            response.Message = $"Failed to update storage: {ex.Message}";
        }

        return response;
    }
}
