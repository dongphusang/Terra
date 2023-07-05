using Terra.ViewModels;

namespace Terra;

public partial class RemoveEmailPage : ContentPage
{
	public RemoveEmailPage()
	{
		InitializeComponent();
		BindingContext = new EmailSubViewModel();
	}
}