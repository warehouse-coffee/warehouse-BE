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
        public Task<ResponseDto> CreateCustomer(ISender sender, CreateEmployeeCommand command)
        {
            return sender.Send(command);
        }
        [Authorize(Roles = "Admin")]
        public Task<ResponseDto> UpdateCustomer(ISender sender, UpdateEmployeeCommand command)
        {
            return sender.Send(command);
        }
        [Authorize(Roles = "Admin")]
        public Task<EmployeeListVM> GetListCustomer(ISender sender, GetListEmployeeQuery query  )
        {
            return sender.Send(query);
        }
        public Task<EmployeeDetailVM> GetCustomerDetail(ISender sender, string id) 
        {
            return sender.Send(new GetEmployeeDetailQuery { Id = id });
        }
        [Authorize(Roles = "Admin")]
        public Task<bool> DeleteCustomer(ISender sender, string id)
        {
            return sender.Send(new DeleteEmployeeCommand { Id = id });
        }
    }
}
