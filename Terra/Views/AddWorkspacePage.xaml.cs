using Terra.ViewModels;

namespace Terra;

public partial class AddWorkspacePage : ContentPage
{
	public AddWorkspacePage()
	{
		InitializeComponent();
		BindingContext = new WorkspaceViewModel(); 
	}



}
