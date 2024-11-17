using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Linq.Dynamic.Core;
using System.Text;
using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Common.Models;
using warehouse_BE.Domain.Enums;

namespace warehouse_BE.Infrastructure.Service;

public class FilterDataService : IFilterData
{
    private readonly ILogger<FilterDataService> _logger;

    public FilterDataService(ILogger<FilterDataService> logger)
    {
        _logger = logger;
    }

    public IQueryable<T> HandleFilterData<T>(IQueryable<T> query, List<FilterData> filterData, string mainTable = "")
    {
        if (query == null)
        {
            _logger.LogWarning("Query is null. Returning an empty result set.");
            return Enumerable.Empty<T>().AsQueryable();
        }

        if (filterData == null || filterData.Count == 0)
        {
            return query;
        }

        try
        {
            if (!string.IsNullOrEmpty(mainTable))
            {
                mainTable += ".";
            }

            var filterBuilder = new StringBuilder();

            foreach (var item in filterData)
            {
                if (string.IsNullOrEmpty(item.Value))
                {
                    continue;
                }

                string sanitizedValue = item.Value.ToString().ToLower().Trim();
                item.Filter = item.Filter.Replace(" ", string.Empty);

                switch (item.Filter)
                {
                    case nameof(FilterTableType.Contains):
                        filterBuilder.AppendFormat(CultureInfo.InvariantCulture, " {0}{1}.Contains(\"{2}\") And ", mainTable, item.Prop, sanitizedValue);
                        break;
                    case nameof(FilterTableType.NotContains):
                        filterBuilder.AppendFormat(CultureInfo.InvariantCulture, " (!{0}{1}.Contains(\"{2}\") Or {0}{1} == null) And ", mainTable, item.Prop, sanitizedValue);
                        break;
                    case nameof(FilterTableType.Equals):
                        if (item.Type == "string")
                        {
                            filterBuilder.AppendFormat(CultureInfo.InvariantCulture, " {0}{1}.Equals(\"{2}\") And ", mainTable, item.Prop, sanitizedValue);
                        }
                        else if (item.Type == "date")
                        {
                            if (DateTime.TryParse(item.Value?.ToString(), new CultureInfo("en-US"), DateTimeStyles.None, out DateTime datetimeFilter))
                            {
                                var startOfDate = datetimeFilter;
                                var endOfDate = datetimeFilter.AddDays(1).AddTicks(-1);
                                filterBuilder.AppendFormat(CultureInfo.InvariantCulture, " ({0}{1} >= \"{2:O}\" And {0}{1} <= \"{3:O}\") And ", mainTable, item.Prop, startOfDate, endOfDate);
                            }
                        }
                        break;
                    case nameof(FilterTableType.NotEquals):
                        if (item.Type == "string")
                        {
                            filterBuilder.AppendFormat(CultureInfo.InvariantCulture, " (!{0}{1}.Equals(\"{2}\") Or {0}{1} == null) And ", mainTable, item.Prop, sanitizedValue);
                        }
                        else if (item.Type == "date")
                        {
                            if (DateTime.TryParse(item.Value?.ToString(), new CultureInfo("en-US"), DateTimeStyles.None, out DateTime datetimeFilter))
                            {
                                var startOfDate = datetimeFilter;
                                var endOfDate = datetimeFilter.AddDays(1).AddTicks(-1);
                                filterBuilder.AppendFormat(CultureInfo.InvariantCulture, " ({0}{1} < \"{2:O}\" Or {0}{1} > \"{3:O}\") And ", mainTable, item.Prop, startOfDate, endOfDate);
                            }
                        }
                        break;
                    default:
                        _logger.LogWarning("Unsupported filter type: {FilterType}", item.Filter);
                        break;
                }
            }

            if (filterBuilder.Length > 4)
            {
                // Remove the trailing " And "
                filterBuilder.Length -= 4;
                query = query.Where(filterBuilder.ToString());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred in HandleFilterData: {Message}", ex.Message);
        }

        return query;
    }

    public IQueryable<T> HandleSort<T>(IQueryable<T> query, string param, bool sortAsc, string mainTable = "")
    {
        if (query == null)
        {
            _logger.LogWarning("Query is null. Returning an empty result set.");
            return Enumerable.Empty<T>().AsQueryable();
        }

        if (string.IsNullOrEmpty(param))
        {
            return query;
        }

        try
        {
            if (!string.IsNullOrEmpty(mainTable))
            {
                mainTable += ".";
            }

            var condition = sortAsc ? $"{mainTable}{param} ASC" : $"{mainTable}{param} DESC";
            query = query.OrderBy(condition);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred in HandleSort: {Message}", ex.Message);
        }

        return query;
    }
}
