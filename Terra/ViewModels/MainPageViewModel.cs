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
        async Task ToAddWorkspacePage() => await Task.Run(() => Shell.Current.GoToAsync(nameof(AddWorkspacePage), false));

        [RelayCommand]
        async Task ToWorkspaceList() => await Task.Run(() => Shell.Current.GoToAsync(nameof(WorkspaceList), false));

        [RelayCommand]
        async Task ToGraphicalView() => await Task.Run(() => Shell.Current.GoToAsync(nameof(GraphicalView), false));

        [RelayCommand]
        async Task ToEmailSubPage() => await Task.Run(() => Shell.Current.GoToAsync(nameof(EmailSubPage), false));

        

    }
}

