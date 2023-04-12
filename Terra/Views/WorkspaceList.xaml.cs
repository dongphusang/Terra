using Terra.ViewModels;

namespace Terra;

public partial class WorkspaceList : ContentPage
{
	private WorkspaceViewModel _viewModel;

	/// <summary>
	/// Code behind constructor. Has to invoke the UpdateWorkspace() method since it's not updating texts upon navigation
	/// </summary>
	public WorkspaceList()
	{
		InitializeComponent();
		_viewModel = new WorkspaceViewModel();
		BindingContext = _viewModel;
        _viewModel.UpdateWorkspace();
    }

}