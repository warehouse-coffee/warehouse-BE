using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Employee.Queries.GetEmployeeDetail;

namespace warehouse_BE.Application.CompanyOwner.Queries.GetCompanyOwnerDetail
{
    public class GetCompanyOwnerDetailQuery : IRequest<CompanyOwnerDetailDto> 
    {
        public required string UserId { get; set; }
    }
    public class GetCompanyOwnerDetailQueryHandler : IRequestHandler<GetCompanyOwnerDetailQuery, CompanyOwnerDetailDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetCompanyOwnerDetailQueryHandler(IApplicationDbContext context, IMapper mapper, IIdentityService identityService, IHttpContextAccessor httpContextAccessor)
        {
            this._context = context;
            this._mapper = mapper;
            this._identityService = identityService;
            this._httpContextAccessor = httpContextAccessor;
        }
        public async Task<CompanyOwnerDetailDto> Handle (GetCompanyOwnerDetailQuery request, CancellationToken cancellationToken)
        {
            var rs = new CompanyOwnerDetailDto();
            rs = await _identityService.GetCompanyOwnerByIdAsync(request.UserId);
            if (rs == null)
            {
                return new CompanyOwnerDetailDto { };
            }
            var company = await _context.Companies
            .FirstOrDefaultAsync(c => c.CompanyId == rs.CompanyId, cancellationToken);

            if (company != null)
            {
                rs.CompanyName = company.CompanyName;
                rs.CompanyPhone = company.PhoneContact;
                rs.CompanyEmail = company.EmailContact;
                rs.CompanyAddress = company.Address;
            }
            if (_httpContextAccessor.HttpContext != null &&
                 _httpContextAccessor.HttpContext.Request != null)
            {
                var requestContext = _httpContextAccessor.HttpContext.Request;
                var scheme = requestContext.Scheme;
                var host = requestContext.Host.Value;
                rs.ImageFile = $"{scheme}://{host}/Resources/{rs.ImageFile}";
            }
           

            
            return rs;
        }
    }
}
