namespace BuhtaLink.Services;

public interface IRestApiService
{
    Task<T> GetAsync<T>(string endpoint);
    Task<T> PostAsync<T>(string endpoint, object data);
    Task<bool> LoginAsync(string username, string password);
    Task<bool> RegisterAsync(string username, string password, string fullName, string nickname);
}