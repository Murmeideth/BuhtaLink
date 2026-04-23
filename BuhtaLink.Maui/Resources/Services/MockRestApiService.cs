// Services/MockRestApiService.cs
namespace BuhtaLink.Services;

public class MockRestApiService : IRestApiService
{
    public async Task<T> GetAsync<T>(string endpoint)
    {
        await Task.Delay(300); // Имитация сети
        return default(T);
    }

    public async Task<T> PostAsync<T>(string endpoint, object data)
    {
        await Task.Delay(300);
        return default(T);
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        await Task.Delay(500);
        return username == "test" && password == "test";
    }
}