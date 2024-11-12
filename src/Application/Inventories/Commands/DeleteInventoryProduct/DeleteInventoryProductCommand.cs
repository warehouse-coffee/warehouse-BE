using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Response;
using warehouse_BE.Domain.Entities;

namespace warehouse_BE.Application.Inventories.Commands.DeleteInventoryProduct;

public class DeleteInventoryProductCommand : IRequest<ResponseDto>
{
    public int Id { get; set; }
}
public class DeleteInventoryProductCommandHandler : IRequestHandler<DeleteInventoryProductCommand, ResponseDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _currentUser;
    private readonly ILoggerService _logger;
    public DeleteInventoryProductCommandHandler(IApplicationDbContext context, IUser currentUser, ILoggerService logger)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
    }
    public async Task<ResponseDto> Handle(DeleteInventoryProductCommand request, CancellationToken cancellationToken)
    {
        var inventory = await _context.Inventories
           .Where(i => i.Id == request.Id)
           .Select(i => new Inventory { Id = i.Id, ProductName = i.ProductName ,IsDeleted = i.IsDeleted }) 
           .FirstOrDefaultAsync(cancellationToken);

        if (inventory == null)
        {
            return new ResponseDto(404, "Inventory not found.");
        }
        try
        {
            inventory.IsDeleted = true;

            _context.Inventories.Update(inventory);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Delete Inventory Product with ID" + request.Id);
        } catch(Exception ex) {
            _logger.LogError("Error at Delete Inventory Product with ID " + request.Id, ex);  
        }
        

        return new ResponseDto(200, "Inventory deleted successfully.", new { Id = request.Id });

    }
}
