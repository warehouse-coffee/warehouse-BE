using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace warehouse_BE.Application.IdentityUser.Commands.SignIn;

public class SignInVm
{
    public string? Token { get; set; }
    //public string? xsrfToken { get; set; }
    public int StatusCode { get; set; }
}
