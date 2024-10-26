using Microsoft.AspNetCore.Authorization;
using warehouse_BE.Application.Customer.Commands.CreateCustomer;
using warehouse_BE.Application.Customer.Commands.DeleteCustomer;
using warehouse_BE.Application.Customer.Commands.UpdateCustomer;
using warehouse_BE.Application.Customer.Queries.GetCustomerDetail;
using warehouse_BE.Application.Customer.Queries.GetListCustomer;
using warehouse_BE.Application.Customer.Queries.GetLlistCustomer;
using warehouse_BE.Application.IdentityUser.Commands.CreateUser;
using warehouse_BE.Application.IdentityUser.Commands.ResetPassword;
using warehouse_BE.Application.IdentityUser.Commands.SignIn;
using warehouse_BE.Application.Response;

namespace warehouse_BE.Web.Endpoints
{
    public class Customers : EndpointGroupBase
    {
        public override void Map(WebApplication app)
        {
            app.MapGroup(this)
                .RequireAuthorization()
                .MapPost(CreateCustomer)
                .MapPut(UpdateCustomer,"")
                .MapPost(GetListCustomer,"/all")
                .MapGet(GetCustomerDetail,"/{id}")
                .MapDelete(DeleteCustomer, "/{id}")
                ;
        }
        [Authorize(Roles = "Admin")]
        public Task<ResponseDto> CreateCustomer(ISender sender, CreateCustomerCommand command)
        {
            return sender.Send(command);
        }
        [Authorize(Roles = "Admin")]
        public Task<ResponseDto> UpdateCustomer(ISender sender, UpdateCustomerCommand command)
        {
            return sender.Send(command);
        }
        [Authorize(Roles = "Admin")]
        public Task<CustomerListVM> GetListCustomer(ISender sender, GetListCustomerQuery query  )
        {
            return sender.Send(query);
        }
        [Authorize(Roles = "Admin")]
        public Task<CustomerDetailVM> GetCustomerDetail(ISender sender, string id) 
        {
            return sender.Send(new GetCustomerDetailQuery { UserId = id });
        }
        [Authorize(Roles = "Admin")]
        public Task<bool> DeleteCustomer(ISender sender, string id)
        {
            return sender.Send(new DeleteCustomerCommand { UserId = id });
        }
    }
}
