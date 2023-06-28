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
        public Task ToAddWorkspacePage() => Shell.Current.GoToAsync(nameof(AddWorkspacePage));

        [RelayCommand]
        public Task ToWorkspaceList() => Shell.Current.GoToAsync(nameof(WorkspaceList));

        [RelayCommand]
        public Task ToGraphicalView() => Shell.Current.GoToAsync(nameof(GraphicalView));

        [RelayCommand]
        public Task ToEmailSubPage() => Shell.Current.GoToAsync(nameof(EmailSubPage));

        

    }
}

