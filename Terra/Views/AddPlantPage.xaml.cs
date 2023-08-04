using Terra.ViewModels;

namespace Terra;

public partial class AddPlantPage : ContentPage
{
    private PlantViewModel _viewModel;

    public AddPlantPage()
	{
        _viewModel = new();
		InitializeComponent();
		BindingContext = _viewModel;
	}

    // query plant api as user enters text
    private async void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {
        string lastProcessed = "";

        // clear last processed text if user removes all text
        if (string.IsNullOrEmpty((sender as Entry).Text)) lastProcessed = null;
        // checks if user is typing (inner method) horrible readability I know :(
        async Task<bool> UserTyping()
        {
            var txt = (sender as Entry).Text;
            await Task.Delay(500);
            return txt != (sender as Entry).Text;
        }
        // as user is typing or current text is the same, do nothing
        if (await UserTyping() || (sender as Entry).Text == lastProcessed) return;
        // save current text and send query to api
        lastProcessed = (sender as Entry).Text;
        await _viewModel.UpdateCollectionView(lastProcessed);
    }
}
