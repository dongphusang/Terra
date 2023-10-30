using Microsoft.Maui.Controls;
using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Terra.ViewModels
{
	public partial class MainPageViewModel : ObservableObject
	{

		public MainPageViewModel()
		{
		}

        [RelayCommand]
        async Task ToAddWorkspacePage() => await Shell.Current.GoToAsync(nameof(AddWorkspacePage), false);

        [RelayCommand]
        async Task ToWorkspaceList() => await Shell.Current.GoToAsync(nameof(WorkspaceList), false);

        [RelayCommand]
        async Task ToGraphicalView() => await Shell.Current.GoToAsync(nameof(GraphicalView), false);

        [RelayCommand]
        async Task ToEmailSubPage() => await Shell.Current.GoToAsync(nameof(EmailSubPage), false);

        

    }
}

