using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Application.Common.Interfaces;

namespace warehouse_BE.Application.Categories.Commands.DeleteCaterogy;

public class DeleteCaterogyCommand : IRequest<bool>
{
    public int Id { get; set; }
}
public class DeleteCaterogyCommandHandler : IRequestHandler<DeleteCaterogyCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ILoggerService _loggerService;
    public DeleteCaterogyCommandHandler(IApplicationDbContext context, ILoggerService loggerService)
    {
        _context = context;
        _loggerService = loggerService;
    }
    public async Task<bool> Handle(DeleteCaterogyCommand request, CancellationToken cancellationToken)
    {
        bool result = false;

        try
        {
            var category = await _context.Categories.FindAsync(new object[] { request.Id }, cancellationToken);

            if (category == null)
            {
                _loggerService.LogWarning($"Category with Id {request.Id} not found.");
            }
            else
            {
                category.IsDeleted = true;

                _context.Categories.Update(category);

                await _context.SaveChangesAsync(cancellationToken);

                result = true; 
            }
        }
        catch (Exception ex)
        {
            _loggerService.LogError($"An error occurred while deleting the category: {ex.Message}", ex);
        }

        return result;
    }
}