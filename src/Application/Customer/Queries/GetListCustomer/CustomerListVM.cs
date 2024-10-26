using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Application.Common.Models;
using warehouse_BE.Domain.Common;

namespace warehouse_BE.Application.Customer.Queries.GetListCustomer;

public class CustomerListVM
{
    public CustomerListVM()
    {
        Customers = new List<UserDto> { new UserDto() };
    }
   public List<UserDto>? Customers { get; set; }
    public Page? Page { get; set; }
}
