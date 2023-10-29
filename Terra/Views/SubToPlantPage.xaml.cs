using Terra.ViewModels;

namespace Terra;

public partial class SubToPlantPage : ContentPage
{
	private EmailSubViewModel _viewModel;
	public SubToPlantPage()
	{
		InitializeComponent();
        _viewModel = new EmailSubViewModel();
        BindingContext = _viewModel;
        Task.Run(_viewModel.UpdateEmails);
	}
}