using Terra.ViewModels;

namespace Terra;

public partial class AddWorkspacePage : ContentPage
{
	public AddWorkspacePage()
	{
		InitializeComponent();
		BindingContext = new AddWorkspaceViewModel(); 
	}

	// Perform action upon loading the page
    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
    }


}
