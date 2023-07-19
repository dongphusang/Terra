
namespace Terra;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

        Routing.RegisterRoute(nameof(AddWorkspacePage), typeof(AddWorkspacePage));
		Routing.RegisterRoute(nameof(WorkspaceList), typeof(WorkspaceList));
		Routing.RegisterRoute(nameof(WorkspaceDisplay), typeof(WorkspaceDisplay));
		Routing.RegisterRoute(nameof(AddPlantPage), typeof(AddPlantPage));
		Routing.RegisterRoute(nameof(EmptyPlantSlot), typeof(EmptyPlantSlot));
		Routing.RegisterRoute(nameof(GraphicalView), typeof(GraphicalView));
		Routing.RegisterRoute(nameof(EmailSubPage), typeof(EmailSubPage));
		Routing.RegisterRoute(nameof(RemoveEmailPage), typeof(RemoveEmailPage));
		Routing.RegisterRoute(nameof(PlantInfoPage), typeof(PlantInfoPage));
    }
}
