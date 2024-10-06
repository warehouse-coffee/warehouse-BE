using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Response;
using warehouse_BE.Domain.Entities;

namespace warehouse_BE.Application.Areas.CreateArea;

public class CreateAreaCommand : IRequest<ResponseDto>
{
    public required string AreaName { get; set; }
}
public class CreateAreaCommandHandler : IRequestHandler<CreateAreaCommand, ResponseDto>
{
    private readonly IApplicationDbContext _context;
    public CreateAreaCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<ResponseDto> Handle(CreateAreaCommand request, CancellationToken cancellationToken)
    {
        var rs = new ResponseDto();
        var areaExists = await _context.Areas
            .AnyAsync(a => a.Name == request.AreaName, cancellationToken);

        if (areaExists)
        {
            rs.StatusCode = 400;
            rs.Message = "Area with the same name already exists.";
            return rs;
        }

        try
        {
            var newArea = new Area
            {
                Name = request.AreaName,
               
            };

            _context.Areas.Add(newArea);
            await _context.SaveChangesAsync(cancellationToken); 

            rs.StatusCode = 201; 
            rs.Message = "Area created successfully.";
        }
        catch (Exception ex)
        {
            rs.StatusCode = 500; 
            rs.Message = "An error occurred while creating the area." + ex.Message;
        }
        return rs;
    }
}
