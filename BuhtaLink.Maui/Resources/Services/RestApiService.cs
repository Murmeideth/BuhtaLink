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
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

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

            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<LoginResponse>(responseBody, _jsonOptions);
                if (result?.Token == null) return false;

                SetAuthToken(result.Token);

                // Временно Preferences вместо SecureStorage
                Preferences.Set("jwt_token", result.Token);
                Preferences.Set("user_id", result.User.Id.ToString());
                Preferences.Set("user_nickname", result.User.Nickname ?? result.User.Username);
                Preferences.Set("user_fullname", result.User.FullName ?? "");
                Preferences.Set("user_avatar", result.User.AvatarUrl ?? "personalplaceholder.jpg");

                return true;
            }

            throw new Exception($"[{response.StatusCode}] {responseBody}");
        }
        catch (Exception ex)
        {
            throw new Exception($"Ошибка запроса: {ex.Message}", ex);
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

                await SecureStorage.SetAsync("user_id", result.User.Id.ToString());
                await SecureStorage.SetAsync("user_nickname", result.User.Nickname ?? result.User.Username);
                await SecureStorage.SetAsync("user_fullname", result.User.FullName ?? "");
                await SecureStorage.SetAsync("user_avatar", result.User.AvatarUrl ?? "personalplaceholder.jpg");

                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    
    public async Task<UserProfileDto> GetProfileAsync()
    {
        return await GetAsync<UserProfileDto>("/api/Profile");
    }

    public async Task<List<FriendDto>> GetFriendsAsync()
    {
        return await GetAsync<List<FriendDto>>("/api/Profile/friends");
    }

    public async Task<List<PostDto>> GetUserPostsAsync(int skip = 0, int take = 20)
    {
        return await GetAsync<List<PostDto>>($"/api/Profile/posts?skip={skip}&take={take}");
    }

    public async Task LogoutAsync()
    {
        _httpClient.DefaultRequestHeaders.Authorization = null;

        Preferences.Remove("jwt_token");
        Preferences.Remove("user_id");
        Preferences.Remove("user_nickname");
        Preferences.Remove("user_fullname");
        Preferences.Remove("user_avatar");

        await Shell.Current.GoToAsync("//login");
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