using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace warehouse_BE.Application.Common.Interfaces;

public interface IExternalHttpService
{
    Task<T?> GetAsync<T>(string endpoint);
    Task<T?> PostAsync<T>(string endpoint, T data);
    Task<T?> PutAsync<T>(string endpoint, T data);
    Task<T?> DeleteAsync<T>(string endpoint);

}

