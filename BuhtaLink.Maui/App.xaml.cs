using BuhtaLink.Services;

namespace BuhtaLink;

public partial class App : Application
{
    public App()
    {
        MainPage = new AppShell();
    }

    protected override async void OnStart()
    {
        base.OnStart();
        await CheckAuthState();
    }

    private async Task CheckAuthState()
    {
        try
        {
            var token = Preferences.Get("jwt_token", "");
            if (string.IsNullOrEmpty(token))
                return;

            // Получаем сервис из DI
            var apiService = IPlatformApplication.Current!.Services.GetService<IRestApiService>();
            if (apiService == null)
                return;

            // Устанавливаем токен
            if (apiService is RestApiService restService)
            {
                restService.SetAuthToken(token);
            }

            // Проверяем, валиден ли токен
            try
            {
                var profile = await apiService.GetProfileAsync();
                if (profile != null)
                {
                    await Shell.Current.GoToAsync("//main");
                }
            }
            catch
            {
                // Токен невалиден — удаляем
                SecureStorage.Remove("jwt_token");
            }
        }
        catch (Exception)
        {
            // Остаёмся на странице логина
        }
    }
}