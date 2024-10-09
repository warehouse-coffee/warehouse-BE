using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using warehouse_BE.Application.CompanyOwner.Commands.CreateCompanyOwner;
using warehouse_BE.Application.CompanyOwner.Commands.DeleteCompanyOwner;
using warehouse_BE.Application.CompanyOwner.Commands.UpdateCompanyOwner;
using warehouse_BE.Application.CompanyOwner.Queries.GetCompanyOwnerDetail;
using warehouse_BE.Application.CompanyOwner.Queries.GetCompanyOwnerList;
using warehouse_BE.Application.IdentityUser.Commands.ResetPassword;
using warehouse_BE.Application.IdentityUser.Commands.SignIn;
using warehouse_BE.Application.Response;

namespace warehouse_BE.Web.Endpoints
{
    public class CompanyOwner : EndpointGroupBase
    {
        public override void Map(WebApplication app)
        {
            app.MapGroup(this).RequireAuthorization()
            .MapPost(Create)
            .MapGet(GetAll, "/all")
            .MapGet(GetDetail, "{id}")
            .MapPut(Update, "{id}")
            .MapDelete(Delete, "{id}")
            ;
        }
        public Task<ResponseDto> Create(ISender sender, CreateCompanyOwnerCommand command)
        {
            return sender.Send(command);
        }
        public Task<CompanyOwnerListVM> GetAll(ISender sender)
        {
            return sender.Send(new GetCompanyOwnerListQuery());
        }
        public Task<CompanyOwnerDetailDto> GetDetail(ISender sender, string id)
        {
            return sender.Send(new GetCompanyOwnerDetailQuery() { UserId = id });
        }
        [Authorize(Roles = "Super-Admin")]
        public Task<ResponseDto> Update(ISender sender, [FromForm] UpdateCompanyOwnerCommand command, string id)
        {
            command.UserId = id;
            return sender.Send(command);
        }
        public Task<bool> Delete(ISender sender, string id)
        {
            return sender.Send(new DeleteCompanyOwnerCommand { UserId = id });
        }
    }
}
