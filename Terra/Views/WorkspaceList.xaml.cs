using Terra.ViewModels;

namespace Terra;

public partial class WorkspaceList : ContentPage
{
	public WorkspaceList()
	{
		InitializeComponent();
		BindingContext = new WorkspaceViewModel();
	}
}