using Terra.ViewModels;

namespace Terra;

public partial class SubToPlantPage : ContentPage
{
	private EmailSubViewModel _viewModel;
	public SubToPlantPage()
	{
		_viewModel = new EmailSubViewModel();
		BindingContext = _viewModel;
		InitializeComponent();
		_viewModel.UpdateEmails();
	}
}