using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Customer.Commands.CreateCustomer;
using warehouse_BE.Application.Customer.Commands.UpdateCustomer;
using warehouse_BE.Application.IdentityUser.Commands.CreateUser;
using warehouse_BE.Application.IdentityUser.Commands.LogOut;
using warehouse_BE.Application.IdentityUser.Commands.ResetPassword;
using warehouse_BE.Application.IdentityUser.Commands.SignIn;
using warehouse_BE.Application.Response;

namespace warehouse_BE.Web.Endpoints;

public class IdentityUser : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(SignIn, "/signin")
            .MapPost(ResetPassword, "/resetpassword")
            .MapPost(Logout, "/logout");
    }
    public Task<SignInVm> SignIn(ISender sender, SignInCommand query)
    {
        return sender.Send(query);
    }
    public Task<ResetPasswordVm> ResetPassword(ISender sender, ResetPasswordCommand command)
    {
        return sender.Send(command);
    }
    public Task<bool> Logout(ISender sender, [FromQuery] string Id)
    {
        return sender.Send(new LogoutCommand { UserId = Id });
    }
}
