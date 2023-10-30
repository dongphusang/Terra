using Terra.ViewModels;

namespace Terra;

public partial class ModeConfigPage : ContentPage
{
	private OperatingModeViewModel _viewModel;

	public ModeConfigPage()
	{
		_viewModel = new();
		BindingContext = _viewModel;
		InitializeComponent();
        Task.Run(_viewModel.UpdateSchedules);
    }
}