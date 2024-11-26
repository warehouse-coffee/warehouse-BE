
using warehouse_BE.Domain.Entities;

namespace warehouse_BE.Application.Customers.Queries.GetListCustomerOfUser;

public class CustomersVM 
{
  public List<CustomerDto> Customers { get; set; } = new List<CustomerDto>();
}
