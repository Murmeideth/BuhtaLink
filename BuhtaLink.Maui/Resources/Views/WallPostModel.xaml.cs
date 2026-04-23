namespace BuhtaLink.Resources.Views;

public partial class WallPostModel : ContentView
{
    public static readonly BindableProperty UserNameProperty =
        BindableProperty.Create(nameof(UserName), typeof(string), typeof(WallPostModel), "Прозвище");

    public static readonly BindableProperty PostTextProperty =
        BindableProperty.Create(nameof(PostText), typeof(string), typeof(WallPostModel),
            "Какая-то туевая куча текста...");

    public static readonly BindableProperty AvatarSourceProperty =
        BindableProperty.Create(nameof(AvatarSource), typeof(string), typeof(WallPostModel),
            "personalplaceholder.jpg");

    public static readonly BindableProperty CommentsCountProperty =
        BindableProperty.Create(nameof(CommentsCount), typeof(int), typeof(WallPostModel), 0);

    public static readonly BindableProperty PostDateProperty =
        BindableProperty.Create(nameof(PostDate), typeof(string), typeof(WallPostModel),
            DateTime.Now.ToString("dd.MM.yyyy"));

    public string UserName
    {
        get => (string)GetValue(UserNameProperty);
        set => SetValue(UserNameProperty, value);
    }

    public string PostText
    {
        get => (string)GetValue(PostTextProperty);
        set => SetValue(PostTextProperty, value);
    }

    public string AvatarSource
    {
        get => (string)GetValue(AvatarSourceProperty);
        set => SetValue(AvatarSourceProperty, value);
    }

    public int CommentsCount
    {
        get => (int)GetValue(CommentsCountProperty);
        set => SetValue(CommentsCountProperty, value);
    }

    public string PostDate
    {
        get => (string)GetValue(PostDateProperty);
        set => SetValue(PostDateProperty, value);
    }

    public WallPostModel()
    {
        InitializeComponent();
    }
}