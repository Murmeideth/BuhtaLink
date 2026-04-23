using BuhtaLink.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace BuhtaLink.ViewModels;

public partial class ProfileViewModel : ObservableObject
{
    private readonly IRestApiService _apiService;

    [ObservableProperty]
    private string _userNickname = "Загрузка...";

    [ObservableProperty]
    private string _userFullName = "";

    [ObservableProperty]
    private string _userAvatar = "personalplaceholder.jpg";

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isRefreshing;

    [ObservableProperty]
    private ObservableCollection<FriendModel> _friends = new();

    [ObservableProperty]
    private ObservableCollection<WallPostData> _wallPosts = new();

    public ProfileViewModel(IRestApiService apiService)
    {
        _apiService = apiService;
        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
        RefreshCommand = new AsyncRelayCommand(RefreshDataAsync);
    }

    public IAsyncRelayCommand LoadDataCommand { get; }
    public IAsyncRelayCommand RefreshCommand { get; }

    private async Task LoadDataAsync()
    {
        // Сначала пробуем загрузить из SecureStorage (мгновенно)
        UserNickname = await SecureStorage.GetAsync("user_nickname") ?? "Пользователь";
        UserFullName = await SecureStorage.GetAsync("user_fullname") ?? "";
        UserAvatar = await SecureStorage.GetAsync("user_avatar") ?? "personalplaceholder.jpg";

        // Затем обновляем с сервера
        await RefreshDataAsync();
    }

    private async Task RefreshDataAsync()
    {
        if (IsLoading) return;

        IsLoading = true;
        IsRefreshing = true;

        try
        {
            // Загружаем профиль
            var profile = await _apiService.GetProfileAsync();
            if (profile != null)
            {
                UserNickname = profile.Nickname ?? profile.Username;
                UserFullName = profile.FullName ?? "";
                UserAvatar = profile.AvatarUrl ?? "personalplaceholder.jpg";

                // Сохраняем актуальные данные
                await SecureStorage.SetAsync("user_nickname", UserNickname);
                await SecureStorage.SetAsync("user_fullname", UserFullName);
                await SecureStorage.SetAsync("user_avatar", UserAvatar);
            }

            // Загружаем друзей
            var friends = await _apiService.GetFriendsAsync();
            if (friends != null)
            {
                Friends = new ObservableCollection<FriendModel>(
                    friends.Select(f => new FriendModel
                    {
                        Name = f.Name,
                        Avatar = f.Avatar
                    }));
            }

            // Загружаем посты
            var posts = await _apiService.GetUserPostsAsync();
            if (posts != null)
            {
                WallPosts = new ObservableCollection<WallPostData>(
                    posts.Select(p => new WallPostData
                    {
                        UserName = p.UserName,
                        PostText = p.PostText,
                        CommentsCount = p.CommentsCount,
                        PostDate = p.PostDate,
                        AvatarSource = p.AvatarSource
                    }));
            }
        }
        catch (Exception)
        {
            // Если сервер недоступен, остаёмся с данными из SecureStorage
        }
        finally
        {
            IsLoading = false;
            IsRefreshing = false;
        }
    }
}

public class FriendModel
{
    public string Name { get; set; }
    public string Avatar { get; set; }
}

public class WallPostData
{
    public string UserName { get; set; }
    public string PostText { get; set; }
    public int CommentsCount { get; set; }
    public string PostDate { get; set; }
    public string AvatarSource { get; set; } = "personalplaceholder.jpg";
}