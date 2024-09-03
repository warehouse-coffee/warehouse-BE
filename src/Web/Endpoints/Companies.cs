using warehouse_BE.Application.Companies.Commands.CreateCompany;
using warehouse_BE.Application.Companies.Queries.GetCompanyList;
using warehouse_BE.Application.Response;
using warehouse_BE.Domain.Entities;

namespace warehouse_BE.Web.Endpoints;

public class Companies : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            //.RequireAuthorization()  // Nếu cần xác thực
            .MapPost(CreateCompany)
            .MapGet(GetCompanyList);
    }

    public async Task<ResponseDto> CreateCompany(ISender sender, CreateCompanyCommand command)
    {
        return await sender.Send(command);

    }

    //[Authorize(Roles = "Super-Admin")]
    public async Task<ResponseDto> GetCompanyList(ISender sender)
    {
        return await sender.Send(new GetCompanyListQuery());
    }
}
