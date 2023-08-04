using Terra.ViewModels;

namespace Terra;

public partial class PlantInfoPage : ContentPage
{
    private PlantViewModel _viewModel; // viewmodel object

    public PlantInfoPage()
	{
		InitializeComponent();
        _viewModel = new PlantViewModel();
        BindingContext = _viewModel;
        _viewModel.GetPlantDataFromAPI();
    }
}