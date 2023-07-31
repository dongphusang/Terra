using Terra.ViewModels;

namespace Terra;

public partial class AddPlantPage : ContentPage
{
    private event EventHandler<TextChangedEventArgs> TextChanged;
    private PlantViewModel _viewModel;

    public AddPlantPage()
	{
        _viewModel = new();
		InitializeComponent();
		BindingContext = _viewModel;
	}

    private void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {
        EventHandler<TextChangedEventArgs> handler = TextChanged;
        handler?.Invoke(this, e);
        _viewModel.GetPickerOptions(e.NewTextValue);
    }
}
