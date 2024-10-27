using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace warehouse_BE.Application.Customer.Commands.UpdateCustomer;

public class UpdateCustomer
{
    public required string CustomerId { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public List<int>? Warehouses { get; set; }
}
