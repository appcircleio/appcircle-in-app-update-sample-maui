
using Microsoft.Extensions.Logging;

namespace AppcircleSampleMaui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        SetEnvironmentVariables();

#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
    
    
    private static void SetEnvironmentVariables()
    {
        Environment.SetEnvironmentVariable("IOS_STORE_SECRET", "your-ios-secret");
        Environment.SetEnvironmentVariable("ANDROID_STORE_SECRET", "your-android-secret");
        Environment.SetEnvironmentVariable("STORE_URL", "https://your-store-url");
        Environment.SetEnvironmentVariable("IOS_PROFILE_ID", "your-ios-profile-id");
        Environment.SetEnvironmentVariable("ANDROID_PROFILE_ID", "your-android-profile-id");
    }
}