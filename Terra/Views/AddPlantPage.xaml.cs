using Terra.ViewModels;

namespace Terra;

public partial class AddPlantPage : ContentPage
{
	public AddPlantPage()
	{
		InitializeComponent();
		BindingContext = new PlantViewModel();
	}
}
