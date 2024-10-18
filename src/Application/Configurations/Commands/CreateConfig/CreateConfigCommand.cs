using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Response;
using warehouse_BE.Domain.Entities;
using warehouse_BE.Domain.ValueObjects;

namespace warehouse_BE.Application.Configurations.Commands.CreateConfig;

public class CreateConfigCommand : IRequest<ResponseDto>
{
    public string? WebServiceUrl { get; set; }
    public string? AIServiceKey { get; set;}
}
public class CreateConfigCommandHandeler : IRequestHandler<CreateConfigCommand, ResponseDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _curentuser;
    private readonly IIdentityService _identityService;
    public CreateConfigCommandHandeler(IApplicationDbContext context, IUser curentuser, IIdentityService identityService)
    {
        this._context = context;
        this._curentuser = curentuser;
        this._identityService = identityService;
    }
    public async Task<ResponseDto> Handle(CreateConfigCommand request, CancellationToken cancellationToken)
    {
       var rs = new ResponseDto();
        if (string.IsNullOrEmpty(_curentuser?.Id))
        {
            rs.StatusCode = 400;
            rs.Message = "User is not authenticated or missing user ID.";
            return rs;
        }
        var role = _identityService.GetRoleNamebyUserId(_curentuser.Id).ToString();
        if (role == "Super-Admin")
        {
            try
            {
                if (!string.IsNullOrEmpty(request.WebServiceUrl))
                {
                    var webServiceConfig = new Configuration
                    {
                        Key = ConfigurationKeys.WebServiceUrl,
                        Value = request.WebServiceUrl
                    };

                    _context.Configurations.Add(webServiceConfig);
                }

                if (!string.IsNullOrEmpty(request.AIServiceKey))
                {
                    var aiServiceConfig = new Configuration
                    {
                        Key = ConfigurationKeys.AIServiceKey,
                        Value = request.AIServiceKey
                    };

                    _context.Configurations.Add(aiServiceConfig);
                }

                await _context.SaveChangesAsync(cancellationToken);

                rs.StatusCode = 200;
                rs.Message = "Configurations created successfully.";
            }
            catch (Exception ex)
            {
                rs.StatusCode = 400;
                rs.Message = "Configurations created unsuccessfully ! " + ex.Message;
            }
        }
        return rs;
    }
}
