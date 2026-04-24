using BuhtaLink.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BuhtaLink.ViewModels;

public partial class RegisterViewModel : ObservableObject
{
    private readonly IRestApiService _apiService;

    [ObservableProperty]
    private string _username;

    [ObservableProperty]
    private string _fullName;

    [ObservableProperty]
    private string _nickname;

    [ObservableProperty]
    private string _password;

    [ObservableProperty]
    private string _confirmPassword;

    [ObservableProperty]
    private string _errorMessage;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool _hasError;

    public bool IsNotBusy => !IsBusy;

    public RegisterViewModel(IRestApiService apiService)
    {
        _apiService = apiService;
    }

    [RelayCommand]
    private async Task RegisterAsync()
    {
        // Сбрасываем ошибки перед валидацией
        HasError = false;
        ErrorMessage = string.Empty;

        // Валидация
        if (string.IsNullOrWhiteSpace(Username) ||
            string.IsNullOrWhiteSpace(Password) ||
            string.IsNullOrWhiteSpace(FullName))
        {
            ErrorMessage = "Заполните обязательные поля (логин, имя, пароль)";
            HasError = true;
            return;
        }

        if (Password != ConfirmPassword)
        {
            ErrorMessage = "Пароли не совпадают";
            HasError = true;
            return;
        }

        if (Password.Length < 4)
        {
            ErrorMessage = "Пароль должен быть не менее 4 символов";
            HasError = true;
            return;
        }

        IsBusy = true;

        try
        {
            var success = await _apiService.RegisterAsync(
                Username,
                Password,
                FullName,
                Nickname ?? Username);

            if (success)
            {
                // Сбрасываем ошибку перед переходом
                HasError = false;
                ErrorMessage = string.Empty;
                await Shell.Current.GoToAsync("//main");
            }
            else
            {
                ErrorMessage = "Ошибка регистрации. Возможно, логин занят.";
                HasError = true;
            }
        }
        catch (Exception)
        {
            ErrorMessage = "Ошибка подключения к серверу";
            HasError = true;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task GoToLoginAsync()
    {
        await Shell.Current.GoToAsync("//login");
    }
}