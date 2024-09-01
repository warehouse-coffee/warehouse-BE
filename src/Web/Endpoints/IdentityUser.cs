using warehouse_BE.Application.IdentityUser.Commands.CreateUser;
using warehouse_BE.Application.IdentityUser.Commands.SignIn;
using warehouse_BE.Application.Response;

namespace warehouse_BE.Web.Endpoints;

public class IdentityUser : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(UserRegister, "/userregister")
            .MapPost(SignIn,"/signin");
    }

    public Task<ResponseDto> UserRegister(ISender sender, CreateUserCommand command)
    {
        return sender.Send(command);
    }

    public Task<SignInVm> SignIn(ISender sender, SignInCommand query)
    {
        return sender.Send(query);
    }

}
