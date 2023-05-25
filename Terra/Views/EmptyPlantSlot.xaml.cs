using Terra.ViewModels;

namespace Terra;

public partial class EmptyPlantSlot : ContentPage
{
	public EmptyPlantSlot()
	{
		InitializeComponent();
        BindingContext = new PlantViewModel();

    }
}
