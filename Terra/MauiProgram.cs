using Microsoft.Extensions.Logging;
using Terra.ViewModels;

namespace Terra;

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
				fonts.AddFont("LDFComicSans.tff", "LDFComicSans");
				fonts.AddFont("LDFComicSansBold.tff", "LDFComicSansBold");

            });

		builder.Services.AddSingleton<MainPage>();
		builder.Services.AddSingleton<MainPageViewModel>();

		builder.Services.AddTransient<AddWorkspacePage>();
		builder.Services.AddTransient<AddWorkspaceViewModel>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
