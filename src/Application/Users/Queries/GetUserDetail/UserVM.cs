using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Application.Storages.Queries.GetStorageList;

namespace warehouse_BE.Application.Users.Queries.GetUserDetail;

public class UserVM
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? CompanyName { get; set; }
    public string? CompanyId { get; set; }
    public string? CompanyPhone { get; set; }
    public string? CompanyEmail { get; set; }
    public string? CompanyAddress { get; set; }
    public List<StorageDto>? Storages { get; set; }
    public string? AvatarImage { get; set; }
}
