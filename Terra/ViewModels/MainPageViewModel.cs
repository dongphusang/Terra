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
        Task ToAddWorkspacePage() => Shell.Current.GoToAsync("AddWorkspacePage");

        [RelayCommand]
        Task ToWorkspaceList() => Shell.Current.GoToAsync("WorkspaceList");

        

    }
}

