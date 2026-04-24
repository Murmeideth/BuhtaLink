using BuhtaLink.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BuhtaLink.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IRestApiService _apiService;

    [ObservableProperty]
    private string _username;

    [ObservableProperty]
    private string _password;

    [ObservableProperty]
    private string _errorMessage;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool _hasError;

    public bool IsNotBusy => !IsBusy;

    public LoginViewModel(IRestApiService apiService)
    {
        _apiService = apiService;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Заполните все поля";
            HasError = true;
            return;
        }

        IsBusy = true;
        HasError = false;

        try
        {
            var success = await _apiService.LoginAsync(Username, Password);
            if (success)
            {
                await Shell.Current.GoToAsync("//main");
            }
            else
            {
                ErrorMessage = "Неверный логин или пароль";
                HasError = true;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка: {ex.Message}";
            HasError = true;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task GoToRegisterAsync()
    {
        await Shell.Current.GoToAsync("//register");
    }
}