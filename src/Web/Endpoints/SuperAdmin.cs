using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using warehouse_BE.Application.CompanyOwner.Commands.DeleteCompanyOwner;
using warehouse_BE.Application.IdentityUser.Commands.CreateUser;
using warehouse_BE.Application.Response;
using warehouse_BE.Application.Stats.Queries.SuperAdmin;
using warehouse_BE.Application.Users.Commands.UpdateUser;
using warehouse_BE.Application.Users.Queries.GetUserDetail;
using warehouse_BE.Application.Users.Queries.GetUserList;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace warehouse_BE.Web.Endpoints
{
    public class SuperAdmin : EndpointGroupBase
    {
        public override void Map(WebApplication app)
        {
            app.MapGroup(this)
                .RequireAuthorization()
                .MapPost(UserRegister, "user")
                .MapPost(GetAllUsers, "user/all")
                .MapGet(GetUserDetail,"user/{id}")
                .MapDelete(DeleteUser, "user/{id}")
                .MapPut(UpdateUser, "user/{id}")
                .MapGet(GetSuperAdminStats,"stats")
                ;
        }
        public Task<ResponseDto> UserRegister(ISender sender, CreateUserCommand command)
        {
            return sender.Send(command);
        }
        public Task<UserListVm> GetAllUsers(ISender sender, GetUserListQuery query)
        {
            return sender.Send(query);
        }
        public Task<UserVM> GetUserDetail(ISender sender, string id)
        {
            var query = new GetUserDetailQuery { userId = id };
            return sender.Send(query);
        }
        public Task<bool> DeleteUser(ISender sender, string id) {
            return sender.Send(new DeleteCompanyOwnerCommand { UserId = id});
        }
        [Authorize(Roles = "Super-Admin")]
        public Task<ResponseDto> UpdateUser(ISender sender, [FromForm] UpdateUserCommand command, string id)
        {
            command.Id = id;
            return sender.Send(command);
        }

        public Task<SuperAdminStatsVM> GetSuperAdminStats(ISender sender)
        {
            return sender.Send(new SuperAdminStatsQuery());
        }
    }

}
