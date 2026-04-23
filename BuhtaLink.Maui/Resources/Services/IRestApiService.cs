namespace BuhtaLink.Services;

public interface IRestApiService
{
    // Базовые методы для HTTP-запросов
    Task<T> GetAsync<T>(string endpoint);
    Task<T> PostAsync<T>(string endpoint, object data);

    // Специфичные методы мессенджера
    Task<bool> LoginAsync(string username, string password);
    //Task<List<Message>> GetMessagesAsync(int dialogId, int skip = 0, int take = 50);
}