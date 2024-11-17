
using warehouse_BE.Application.Common.Models;

namespace warehouse_BE.Application.Common.Interfaces;

public interface IFilterData
{
     IQueryable<T> HandleFilterData<T>(IQueryable<T> query, List<FilterData> filterData, string mainTable = "");

     IQueryable<T> HandleSort<T>(IQueryable<T> query, string param, bool sortAsc, string mainTable = "");
}
