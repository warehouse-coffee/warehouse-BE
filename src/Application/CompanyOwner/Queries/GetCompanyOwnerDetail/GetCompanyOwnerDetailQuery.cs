﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Customer.Queries.GetCustomerDetail;

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

        public GetCompanyOwnerDetailQueryHandler(IApplicationDbContext context, IMapper mapper, IIdentityService identityService)
        {
            _context = context;
            _mapper = mapper;
            _identityService = identityService;
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
            return rs;
        }
    }
}