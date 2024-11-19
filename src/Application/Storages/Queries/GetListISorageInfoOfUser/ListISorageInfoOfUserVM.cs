using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace warehouse_BE.Application.Storages.Queries.GetListISorageInfoOfUser;

public class ListISorageInfoOfUserVM
{
    public ListISorageInfoOfUserVM()
    {
        Storages = new List<StorageName>() { };
    }
    public List<StorageName>? Storages { get; set; }
}
