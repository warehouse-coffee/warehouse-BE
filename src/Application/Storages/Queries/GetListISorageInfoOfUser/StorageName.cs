using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Domain.Entities;

namespace warehouse_BE.Application.Storages.Queries.GetListISorageInfoOfUser;

public class StorageName
{
    public int Id { get; set; }
    public string? Name { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Storage, StorageName>();
            

        }

    }
}
