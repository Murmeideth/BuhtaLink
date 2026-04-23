using BuhtaLink.Resources.Views;
using BuhtaLink.Services;
using BuhtaLink.ViewModels;
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

        // Регистрация сервисов
        builder.Services.AddSingleton<IRestApiService, MockRestApiService>();

        // Регистрация ViewModel
        builder.Services.AddTransient<ProfileViewModel>();

        // Регистрация страниц
        builder.Services.AddTransient<ProfilePage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}