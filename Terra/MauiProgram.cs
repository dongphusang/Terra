using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using SkiaSharp.Views.Maui.Controls.Hosting;
using Terra.ViewModels;
using Terra.Interfaces;
using Terra.Services;

namespace Terra;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseSkiaSharp(true)
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");

            });

		// Main Page
		builder.Services.AddSingleton<MainPage>();
		builder.Services.AddSingleton<MainPageViewModel>();

		// View Models
		builder.Services.AddTransient<GraphicalPlantViewModel>();
        builder.Services.AddTransient<WorkspaceViewModel>();
		builder.Services.AddTransient<PlantViewModel>();
		builder.Services.AddTransient<EmailSubViewModel>();
		builder.Services.AddTransient<OperatingModeViewModel>();

		// Views
        builder.Services.AddTransient<AddWorkspacePage>();
		builder.Services.AddTransient<WorkspaceList>();
		builder.Services.AddTransient<WorkspaceDisplay>();
		builder.Services.AddTransient<AddPlantPage>();
		builder.Services.AddTransient<EmptyPlantSlot>();
		builder.Services.AddTransient<GraphicalView>();
		builder.Services.AddTransient<EmailSubPage>();
		builder.Services.AddTransient<PlantInfoPage>();
		builder.Services.AddTransient<EmailSubPage>();
		builder.Services.AddTransient<IPopupService, PopupService>();
		builder.Services.AddTransient<ModeConfigPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
