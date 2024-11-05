using Microsoft.AspNetCore.Authorization;
using warehouse_BE.Application.Employee.Commands.CreateEmployee;
using warehouse_BE.Application.Employee.Commands.DeleteEmployee;
using warehouse_BE.Application.Employee.Commands.UpdateEmployee;
using warehouse_BE.Application.Employee.Queries.GetEmployeeDetail;
using warehouse_BE.Application.Employee.Queries.GetListEmployee;
using warehouse_BE.Application.Response;

namespace warehouse_BE.Web.Endpoints
{
    public class Employees : EndpointGroupBase
    {
        public override void Map(WebApplication app)
        {
            app.MapGroup(this)
                .RequireAuthorization()
                .MapPost(CreateEmployee)
                .MapPut(UpdateEmployee,"")
                .MapPost(GetListEmployee,"/all")
                .MapGet(GetEmployeeDetail,"/{id}")
                .MapDelete(DeleteEmployee, "/{id}")
                ;
        }
        [Authorize(Roles = "Admin")]
        public Task<ResponseDto> CreateEmployee(ISender sender, CreateEmployeeCommand command)
        {
            return sender.Send(command);
        }
        [Authorize(Roles = "Admin")]
        public Task<ResponseDto> UpdateEmployee(ISender sender, UpdateEmployeeCommand command)
        {
            return sender.Send(command);
        }
        [Authorize(Roles = "Admin")]
        public Task<EmployeeListVM> GetListEmployee(ISender sender, GetListEmployeeQuery query  )
        {
            return sender.Send(query);
        }
        public Task<EmployeeDetailVM> GetEmployeeDetail(ISender sender, string id) 
        {
            return sender.Send(new GetEmployeeDetailQuery { Id = id });
        }
        [Authorize(Roles = "Admin")]
        public Task<bool> DeleteEmployee(ISender sender, string id)
        {
            return sender.Send(new DeleteEmployeeCommand { Id = id });
        }
    }
}
