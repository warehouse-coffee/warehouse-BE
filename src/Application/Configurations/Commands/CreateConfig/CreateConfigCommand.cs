using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Response;

namespace warehouse_BE.Application.Configurations.Commands.CreateConfig;

public class CreateConfigCommand : IRequest<ResponseDto>
{
    public string? WebServiceUrl { get; set; }
    public string? AIServiceKey { get; set;}
}
public class CreateConfigCommandHandeler : IRequestHandler<CreateConfigCommand, ResponseDto>
{
    private readonly IApplicationDbContext _context;
    public CreateConfigCommandHandeler(IApplicationDbContext context)
    {
        this._context = context;
    }
    public async Task<ResponseDto> Handle(CreateConfigCommand request, CancellationToken cancellationToken)
    {
       var rs = new ResponseDto();
        var data = await _context.Companies.FindAsync();
        return rs;
    }
}
