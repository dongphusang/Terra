using Terra.ViewModels;

namespace Terra;

public partial class MainPage : ContentPage
{

	public MainPage()
	{
		InitializeComponent();
		BindingContext = new MainPageViewModel();
	}

    

}

