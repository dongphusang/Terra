namespace Terra;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

        Routing.RegisterRoute(nameof(AddWorkspacePage), typeof(AddWorkspacePage));
		Routing.RegisterRoute(nameof(WorkspaceList), typeof(WorkspaceList));
		Routing.RegisterRoute(nameof(WorkspaceDisplay), typeof(WorkspaceDisplay));
    }
}
