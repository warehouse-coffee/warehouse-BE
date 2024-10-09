using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Application.Storages.Queries.GetStorageList;

namespace warehouse_BE.Application.CompanyOwner.Queries.GetCompanyOwnerDetail
{
    public class CompanyOwnerDetailDto 
    {
        public string? CompayOwnerId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyId { get; set; }
        public string? CompanyPhone { get; set; }
        public string? CompanyEmail { get; set; }
        public string? CompanyAddress { get; set; }
        public List<StorageDto>? Storages { get; set; }
        public string? ImageFile { get; set; }
    }
}
