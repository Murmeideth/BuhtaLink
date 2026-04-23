using System.Windows.Input;

namespace BuhtaLink.Resources.Views;

public partial class FriendPhoroModel : ContentView
{
    // Основные свойства
    public static readonly BindableProperty FriendIdProperty =
        BindableProperty.Create(nameof(FriendId), typeof(int), typeof(FriendPhoroModel), 0);

    public static readonly BindableProperty FriendNameProperty =
        BindableProperty.Create(nameof(FriendName), typeof(string), typeof(FriendPhoroModel), "Имя друга");

    public static readonly BindableProperty FriendAvatarProperty =
        BindableProperty.Create(nameof(FriendAvatar), typeof(string), typeof(FriendPhoroModel), "personalplaceholder.jpg");
    
    // Команда для нажатия
    public static readonly BindableProperty FriendTapCommandProperty =
        BindableProperty.Create(nameof(FriendTapCommand), typeof(ICommand), typeof(FriendPhoroModel));

    public int FriendId
    {
        get => (int)GetValue(FriendIdProperty);
        set => SetValue(FriendIdProperty, value);
    }

    public string FriendName
    {
        get => (string)GetValue(FriendNameProperty);
        set => SetValue(FriendNameProperty, value);
    }

    public string FriendAvatar
    {
        get => (string)GetValue(FriendAvatarProperty);
        set => SetValue(FriendAvatarProperty, value);
    }

    public ICommand FriendTapCommand
    {
        get => (ICommand)GetValue(FriendTapCommandProperty);
        set => SetValue(FriendTapCommandProperty, value);
    }

    public FriendPhoroModel()
    {
        InitializeComponent();
    }
}