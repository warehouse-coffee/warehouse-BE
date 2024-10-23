using System.Text;
using Newtonsoft.Json; 
using warehouse_BE.Application.Common.Interfaces;

namespace warehouse_BE.Infrastructure.Service;

public class ExternalHttpService : IExternalHttpService
{
    private readonly HttpClient _httpClient;

    public ExternalHttpService()
    {
        _httpClient = new HttpClient();
    }

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        var response = await _httpClient.GetAsync(endpoint);
        response.EnsureSuccessStatusCode();
        var jsonResponse = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(jsonResponse);
    }

    public async Task<T?> PostAsync<T>(string endpoint, T data)
    {
        var json = JsonConvert.SerializeObject(data); 
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(endpoint, content);
        response.EnsureSuccessStatusCode();
        var jsonResponse = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(jsonResponse); 
    }

    public async Task<T?> PutAsync<T>(string endpoint, T data)
    {
        var json = JsonConvert.SerializeObject(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync(endpoint, content);
        response.EnsureSuccessStatusCode();
        var jsonResponse = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(jsonResponse); 
    }

    public async Task<T?> DeleteAsync<T>(string endpoint)
    {
        var response = await _httpClient.DeleteAsync(endpoint);
        response.EnsureSuccessStatusCode();
        var jsonResponse = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(jsonResponse); 
    }
}
