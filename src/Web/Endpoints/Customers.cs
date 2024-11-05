using Microsoft.AspNetCore.Authorization;
using warehouse_BE.Application.Customers.Commands.CreateCustomer;
using warehouse_BE.Application.Employee.Commands.UpdateEmployee;
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
                ;
        }
        [Authorize(Roles = "Admin")]
        public Task<ResponseDto> CreateCustomer(ISender sender, CreateCustomerCommand command)
        {
            return sender.Send(command);
        }
    }
}
