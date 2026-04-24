using BuhtaLink.Services;
using BuhtaLink.ViewModels;
using BuhtaLink.Resources.Views;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace BuhtaLink;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // HttpClient для API
        #if ANDROID
                var baseUrl = "http://10.0.2.2:5082";
#elif WINDOWS
                var baseUrl = "http://localhost:5082"; 
#else
                var baseUrl = "http://localhost:5082"; 
#endif

        builder.Services.AddSingleton<IRestApiService, RestApiService>(sp =>
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(baseUrl);
            httpClient.Timeout = TimeSpan.FromSeconds(30);

            return new RestApiService(httpClient);
        });

        // ViewModel
        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<LoginViewModel>(); 
        builder.Services.AddTransient<RegisterViewModel>();
        // Страницы
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}