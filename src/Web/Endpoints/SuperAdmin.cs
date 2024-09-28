using warehouse_BE.Application.IdentityUser.Commands.CreateUser;
using warehouse_BE.Application.Response;

namespace warehouse_BE.Web.Endpoints
{
    public class SuperAdmin : EndpointGroupBase
    {
        public override void Map(WebApplication app)
        {
            app.MapGroup(this)
                .MapPost(UserRegister, "/userregister")
                ;
        }
        public Task<ResponseDto> UserRegister(ISender sender, CreateUserCommand command)
        {
            return sender.Send(command);
        }
    }

}
