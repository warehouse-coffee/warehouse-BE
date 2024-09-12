using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace warehouse_BE.Application.IdentityUser.Commands.ResetPassword;

public class ResetPasswordVm
{
    public string Token { get; set; } = string.Empty;
    public int StatusCode { get; set; }
}
