using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Response;
using warehouse_BE.Domain.Entities;
using warehouse_BE.Domain.ValueObjects;

namespace warehouse_BE.Application.Configurations.Commands.CreateConfig;

public class CreateConfigCommand : IRequest<ResponseDto>
{
    //public string? WebServiceUrl { get; set; }
    public string? AIServiceKey { get; set;}
    public string? EmailServiceKey { get; set;}
    public string? AiDriverServer { get; set; }
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

        var role = await _identityService.GetRoleNamebyUserId(_curentuser.Id);

        if (role == "Super-Admin")
        {
            try
            {
                var existingConfigs = await _context.Configurations
                    .Where(c => c.Key == ConfigurationKeys.WebServiceUrl ||
                                 c.Key == ConfigurationKeys.AIServiceKey ||
                                 c.Key == ConfigurationKeys.EmailServiceKey ||
                                 c.Key == ConfigurationKeys.AiDriverServer)
                    .ToListAsync(cancellationToken);

                //if (!string.IsNullOrEmpty(request.WebServiceUrl))
                //{
                //    var webServiceConfig = existingConfigs.FirstOrDefault(c => c.Key == ConfigurationKeys.WebServiceUrl);
                //    if (webServiceConfig != null)
                //    {
                //        webServiceConfig.Value = request.WebServiceUrl;
                //    }
                //    else
                //    {
                //        _context.Configurations.Add(new Configuration
                //        {
                //            Key = ConfigurationKeys.WebServiceUrl,
                //            Value = request.WebServiceUrl
                //        });
                //    }
                //}

                if (!string.IsNullOrEmpty(request.AIServiceKey))
                {
                    var aiServiceConfig = existingConfigs.FirstOrDefault(c => c.Key == ConfigurationKeys.AIServiceKey);
                    if (aiServiceConfig != null)
                    {
                        aiServiceConfig.Value = request.AIServiceKey;
                    }
                    else
                    {
                        _context.Configurations.Add(new Configuration
                        {
                            Key = ConfigurationKeys.AIServiceKey,
                            Value = request.AIServiceKey
                        });
                    }
                }

                if (!string.IsNullOrEmpty(request.EmailServiceKey))
                {
                    var emailServiceConfig = existingConfigs.FirstOrDefault(c => c.Key == ConfigurationKeys.EmailServiceKey);
                    if (emailServiceConfig != null)
                    {
                        emailServiceConfig.Value = request.EmailServiceKey;
                    }
                    else
                    {
                        _context.Configurations.Add(new Configuration
                        {
                            Key = ConfigurationKeys.EmailServiceKey,
                            Value = request.EmailServiceKey
                        });
                    }
                }
                if (!string.IsNullOrEmpty(request.AiDriverServer))
                {
                    var aiDriverConfig = existingConfigs.FirstOrDefault(c => c.Key == ConfigurationKeys.AiDriverServer);
                    if (aiDriverConfig != null)
                    {
                        aiDriverConfig.Value = request.AiDriverServer;
                    }
                    else
                    {
                        _context.Configurations.Add(new Configuration
                        {
                            Key = ConfigurationKeys.AiDriverServer,
                            Value = request.AiDriverServer
                        });
                    }
                }

                await _context.SaveChangesAsync(cancellationToken);

                rs.StatusCode = 200;
                rs.Message = "Configurations created/updated successfully.";
            }
            catch (Exception ex)
            {
                rs.StatusCode = 400;
                rs.Message = "Configurations created unsuccessfully! " + ex.Message;
            }
        }
        return rs;
    }

}
