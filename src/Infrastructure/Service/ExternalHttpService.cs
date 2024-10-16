using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Application.Common.Interfaces;

namespace warehouse_BE.Infrastructure.Service;

public class ExternalHttpService : IExternalHttpService
{
    private readonly HttpClient _httpClient;

    public ExternalHttpService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<T> GetAsync<T>(string url, IDictionary<string, string>? queryParams = null)
    {
        var fullUrl = queryParams != null
            ? QueryHelpers.AddQueryString(url, queryParams.ToDictionary(kv => kv.Key, kv => (string?)kv.Value))
            : url;

        var response = await _httpClient.GetAsync(fullUrl);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<T>();

        return result ?? throw new Exception("Response content is null or cannot be deserialized.");

    }

    public async Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest data)
    {
        var response = await _httpClient.PostAsJsonAsync(url, data);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<TResponse>();

        return result ?? throw new Exception("Response content is null or cannot be deserialized.");

    }

    public async Task<TResponse> PutAsync<TRequest, TResponse>(string url, TRequest data)
    {
        var response = await _httpClient.PutAsJsonAsync(url, data);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<TResponse>();

        return result ?? throw new Exception("Response content is null or cannot be deserialized.");

    }

    public async Task DeleteAsync(string url)
    {
        var response = await _httpClient.DeleteAsync(url);
        response.EnsureSuccessStatusCode();
    }
}
