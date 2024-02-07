using Terra.ViewModels;

namespace Terra;

public partial class MainPage : ContentPage
{
	MainPageViewModel _viewModel;
	public MainPage()
	{
		InitializeComponent();
		_viewModel = new MainPageViewModel();
		BindingContext = _viewModel;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
		_viewModel.UpdateEventsOnAppearing();
    }
}

