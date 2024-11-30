using warehouse_BE.Application.Companies.Commands.CreateCompany;
using warehouse_BE.Application.Companies.Commands.DeleteCompany;
using warehouse_BE.Application.Companies.Commands.UpdateCompany;
using warehouse_BE.Application.Companies.Queries.CompanyDetail;
using warehouse_BE.Application.Companies.Queries.GetCompanyList;
using warehouse_BE.Application.Response;

namespace warehouse_BE.Web.Endpoints;

public class Companies : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapPost(CreateCompany)
            .MapGet(GetCompanyList)
            .MapGet(GetCompanyDetail, "/{companyId}")
            .MapPut(UpdateCompany, "/{companyId}")
            .MapDelete(DeleteCompany, "/{companyId}");
        ;
    }

    public async Task<ResponseDto> CreateCompany(ISender sender, CreateCompanyCommand command)
    {
        return await sender.Send(command);

    }

    public async Task<CompanyListVM> GetCompanyList(ISender sender)
    {
        return await sender.Send(new GetCompanyListQuery());
    }
    public async Task<ResponseDto> GetCompanyDetail(ISender sender, string companyId)
    {
        var query = new GetCompanyDetailQuery { CompanyId = companyId };
        return await sender.Send(query);
    }

    public async Task<ResponseDto> UpdateCompany(ISender sender, string companyId, UpdateCompanyCommand command)
    {
        var updatedCommand = command with { CompanyId = companyId };

        return await sender.Send(updatedCommand);
    }

    public async Task<ResponseDto> DeleteCompany(ISender sender, string companyId)
    {
        var command = new DeleteCompanyCommand { CompanyId = companyId };
        return await sender.Send(command);
    }
}
