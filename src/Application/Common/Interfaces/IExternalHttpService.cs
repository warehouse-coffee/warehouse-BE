using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace warehouse_BE.Application.Common.Interfaces;

public interface IExternalHttpService
{
    Task<T> GetAsync<T>(string url, IDictionary<string, string>? queryParams = null);
    Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest data);
    Task<TResponse> PutAsync<TRequest, TResponse>(string url, TRequest data);
    Task DeleteAsync(string url);

}

