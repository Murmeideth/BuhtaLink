using BuhtaLink.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace BuhtaLink.ViewModels;

public partial class ProfileViewModel : ObservableObject
{
    private readonly IRestApiService _apiService;

    [ObservableProperty]
    private string _userNickname = "Прозвище человечека";

    [ObservableProperty]
    private string _userFullName = "Имя человечека";

    [ObservableProperty]
    private string _userAvatar = "personalplaceholder.jpg";

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private ObservableCollection<FriendModel> _friends;

    [ObservableProperty]
    private ObservableCollection<WallPostData> _wallPosts;

    public ProfileViewModel(IRestApiService apiService)
    {
        _apiService = apiService;
        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
    }

    public IAsyncRelayCommand LoadDataCommand { get; }

    private async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            await Task.Delay(500);

            Friends = new ObservableCollection<FriendModel>
            {
                new() { Name = "Алексей", Avatar = "personalplaceholder.jpg" },
                new() { Name = "Мария", Avatar = "personalplaceholder.jpg" },
                new() { Name = "Дмитрий", Avatar = "personalplaceholder.jpg" },
                new() { Name = "Екатерина", Avatar = "personalplaceholder.jpg" }
            };

            WallPosts = new ObservableCollection<WallPostData>
            {
                new() { UserName = "Иван Петров", PostText = "Пример текста поста...", CommentsCount = 12, PostDate = "Сегодня, 15:30" },
                new() { UserName = "Анна Смирнова", PostText = "Еще один пример поста...", CommentsCount = 5, PostDate = "Вчера, 10:15" }
            };
        }
        finally
        {
            IsLoading = false;
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