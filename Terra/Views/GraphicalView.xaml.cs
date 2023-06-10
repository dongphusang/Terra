using Terra.ViewModels;

namespace Terra;

public partial class GraphicalView : ContentPage
{
	public GraphicalView()
	{
		InitializeComponent();
        BindingContext = new PlantViewModel();
    }
}
