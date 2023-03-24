using Terra.ViewModels;

namespace Terra;

public partial class WorkspaceList : ContentPage
{
	private WorkspaceViewModel _viewModel;

	public WorkspaceList()
	{
		InitializeComponent();
		_viewModel = new WorkspaceViewModel();
		BindingContext = _viewModel;
		
	}

	// get and setup available workspaces for listing
	protected override void OnAppearing()
	{
		base.OnAppearing();
		_viewModel.PullWorkspaces();
	}
}