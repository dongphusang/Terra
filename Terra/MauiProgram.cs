using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using Terra.ViewModels;

namespace Terra;

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

		builder.Services.AddSingleton<MainPage>();
		builder.Services.AddSingleton<MainPageViewModel>();

		builder.Services.AddTransient<AddWorkspacePage>();
		builder.Services.AddTransient<WorkspaceList>();
		builder.Services.AddTransient<WorkspaceDisplay>();
		builder.Services.AddTransient<WorkspaceViewModel>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
