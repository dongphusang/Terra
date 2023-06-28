using Terra.ViewModels;

namespace Terra;

public partial class EmailSubPage : ContentPage
{
	public EmailSubPage()
	{
		InitializeComponent();
		BindingContext = new EmailSubViewModel();
	}
}