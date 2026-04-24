namespace BuhtaLink.Services;

public interface IRestApiService
{
    Task<T> GetAsync<T>(string endpoint);
    Task<T> PostAsync<T>(string endpoint, object data);
    Task<bool> LoginAsync(string username, string password);
    Task<bool> RegisterAsync(string username, string password, string fullName, string nickname);
    Task LogoutAsync();

    // Профиль
    Task<UserProfileDto> GetProfileAsync();
    Task<List<FriendDto>> GetFriendsAsync();
    Task<List<PostDto>> GetUserPostsAsync(int skip = 0, int take = 20);
}

// DTO модели (добавить в конец файла)
public class UserProfileDto
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string FullName { get; set; }
    public string Nickname { get; set; }
    public string AvatarUrl { get; set; }
    public DateTime? LastSeen { get; set; }
}

public class FriendDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Avatar { get; set; }
}

public class PostDto
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string PostText { get; set; }
    public int CommentsCount { get; set; }
    public string PostDate { get; set; }
    public string AvatarSource { get; set; }
}