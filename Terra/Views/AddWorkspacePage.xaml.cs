using Terra.ViewModels;

namespace Terra;

public partial class AddWorkspacePage : ContentPage
{
	WorkspaceViewModel _viewModel;

	public AddWorkspacePage()
	{
		_viewModel = new WorkspaceViewModel();
		InitializeComponent();
		BindingContext = _viewModel;
	}



}
