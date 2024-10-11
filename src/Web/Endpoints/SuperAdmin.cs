using warehouse_BE.Application.CompanyOwner.Commands.DeleteCompanyOwner;
using warehouse_BE.Application.IdentityUser.Commands.CreateUser;
using warehouse_BE.Application.Response;
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
                .MapPost(UserRegister, "user")
                .MapGet(GetAllUsers, "user/all")
                .MapGet(GetUserDetail,"user/{id}")
                .MapDelete(DeleteUser, "user/{id}")
                .MapPut(UpdateUser, "user/{id}")
                ;
        }
        public Task<ResponseDto> UserRegister(ISender sender, CreateUserCommand command)
        {
            return sender.Send(command);
        }
        public Task<UserListVm> GetAllUsers(ISender sender)
        {
            return sender.Send(new GetUserListQuery());
        }
        public Task<UserVM> GetUserDetail(ISender sender, string id)
        {
            var query = new GetUserDetailQuery { userId = id };
            return sender.Send(query);
        }
        public Task<bool> DeleteUser(ISender sender, string id) {
            return sender.Send(new DeleteCompanyOwnerCommand { UserId = id});
        }
        public Task<ResponseDto> UpdateUser(ISender sender, UpdateUserCommand command)
        {
            return sender.Send(command);
        }
    }

}
