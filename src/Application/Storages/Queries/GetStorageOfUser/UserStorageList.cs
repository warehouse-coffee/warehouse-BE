using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Application.Common.Models;

namespace warehouse_BE.Application.Storages.Queries.GetStorageOfUser;

public class UserStorageList
{
   public UserStorageList()
    {
        Storages = new List<StorageDto> { new StorageDto() };
    }
    public List<StorageDto>? Storages { get; set; }
    public Page? Page { get; set; }
}
