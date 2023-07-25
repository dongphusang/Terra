using CommunityToolkit.Mvvm.ComponentModel;
using Terra.ViewModels;

namespace Terra;

public partial class EmailSubPage : ContentPage
{
	private EmailSubViewModel _viewModel;

	public EmailSubPage()
	{
		InitializeComponent();
		_viewModel = new();
		BindingContext = _viewModel;
		_viewModel.UpdateEmails();
	}
}