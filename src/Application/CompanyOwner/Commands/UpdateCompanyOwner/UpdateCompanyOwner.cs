using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace warehouse_BE.Application.CompanyOwner.Commands.UpdateCompanyOwner
{
    public class UpdateCompanyOwner
    {
        public required string UserId { get; set; }       
        public string? UserName { get; set; }              
        public string? Password { get; set; }             
        public string? Email { get; set; }               
        public string? PhoneNumber { get; set; }          
        public string? CompanyId { get; set; }
    }
}
