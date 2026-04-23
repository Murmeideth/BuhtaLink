using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace BuhtaLink.Services;

public class RestApiService : IRestApiService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public RestApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        // _httpClient.BaseAddress уже установлен в MauiProgram.cs через AddHttpClient
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    // Установка JWT токена после логина
    public void SetAuthToken(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<T> GetAsync<T>(string endpoint)
    {
        var response = await _httpClient.GetAsync(endpoint);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
    }

    public async Task<T> PostAsync<T>(string endpoint, object data)
    {
        var json = JsonSerializer.Serialize(data, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(endpoint, content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        try
        {
            var loginData = new { username, password };
            var response = await _httpClient.PostAsJsonAsync("/api/Auth/login", loginData, _jsonOptions);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>(_jsonOptions);
                SetAuthToken(result.Token);
                await SecureStorage.SetAsync("jwt_token", result.Token);
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> RegisterAsync(string username, string password, string fullName, string nickname)
    {
        try
        {
            var registerData = new { username, password, fullName, nickname };
            var response = await _httpClient.PostAsJsonAsync("/api/Auth/register", registerData, _jsonOptions);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>(_jsonOptions);
                SetAuthToken(result.Token);
                await SecureStorage.SetAsync("jwt_token", result.Token);
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    private class LoginResponse
    {
        public string Token { get; set; }
        public UserDto User { get; set; }
    }

    private class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Nickname { get; set; }
        public string AvatarUrl { get; set; }
        public DateTime? LastSeen { get; set; }
    }
}