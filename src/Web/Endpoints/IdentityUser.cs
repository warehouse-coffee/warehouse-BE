using warehouse_BE.Application.IdentityUser.Commands.CreateUser;
using warehouse_BE.Application.IdentityUser.Commands.SignIn;

namespace warehouse_BE.Web.Endpoints;

public class IdentityUser : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(SignUp,"/signup")
            .MapPost(SignIn,"/signin");
    }

    public Task<int> SignUp(ISender sender, CreateUserCommand command)
    {
        return sender.Send(command);
    }

    public Task<SignInVm> SignIn(ISender sender, SignInCommand query)
    {
        return sender.Send(query);
    }

}
