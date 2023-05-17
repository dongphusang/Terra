using Terra.ViewModels;

namespace Terra;

public partial class WorkspaceDisplay : ContentPage
{
	private WorkspaceViewModel _viewModel;

	public WorkspaceDisplay()
	{
		InitializeComponent();
		_viewModel = new WorkspaceViewModel();
		BindingContext = _viewModel;
	}
}