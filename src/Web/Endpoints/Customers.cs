using Microsoft.AspNetCore.Authorization;
using warehouse_BE.Application.Customers.Commands.CreateCustomer;
using warehouse_BE.Application.Customers.Commands.DeleteCustomer;
using warehouse_BE.Application.Customers.Commands.UpdateCustomer;
using warehouse_BE.Application.Customers.Queries.CustomerDetail;
using warehouse_BE.Application.Customers.Queries.GetListCustomerOfUser;
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
                .MapGet(GetCustomerOfCompany,"company")
                .MapDelete(DeleteCustomer,"/{id:int}")
                .MapPut(UpdateCustomer,"/{id:int}")
                .MapGet(GetCustomerDetail)
            ;
        }
        public Task<ResponseDto> CreateCustomer(ISender sender, CreateCustomerCommand command)
        {
            return sender.Send(command);
        }
        public Task<CustomersVM> GetCustomerOfCompany(ISender sender)
        {
            return sender.Send(new GetListCustomerOfUserQuery());
        }
        public Task<ResponseDto> DeleteCustomer(ISender sender, int id)
        {
            var command = new DeleteCustomerCommand { Id = id };
            return sender.Send(command);
        }

        public Task<ResponseDto> UpdateCustomer(ISender sender, int id, UpdateCustomerCommand command)
        {
            command.CustomerId = id;
            return sender.Send(command);
        }
        public async Task<CustomerDetailDto> GetCustomerDetail(ISender sender, int id, ILogger<Customers> logger)
        {
            try
            {
                logger.LogInformation($"[API] Fetching details for customer ID: {id}");
                var query = new GetCustomerDetailQuery { CustomerId = id };
                var result = await sender.Send(query);
                logger.LogInformation($"[API] Successfully fetched customer details for ID: {id}");
                return result;
            }
            catch (KeyNotFoundException ex)
            {
                logger.LogWarning($"[API] Customer not found: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError($"[API] Unexpected error fetching customer ID {id}: {ex}");
                throw new ApplicationException("An error occurred while fetching customer details.", ex);
            }
        }
    }
}
