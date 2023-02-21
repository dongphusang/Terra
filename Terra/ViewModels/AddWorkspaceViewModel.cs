using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Terra.Models;
using Terra.Services;

namespace Terra.ViewModels
{
    public partial class AddWorkspaceViewModel : ObservableObject
    {
        [ObservableProperty]
        public Workspace workspace;    

        public AddWorkspaceViewModel()
        {
            Workspace = new();
        }

        [RelayCommand]
        async void PostWorkspace()
        {
            await WorkspaceService.LoadMauiAsset();
            WorkspaceService.InsertToTable("workspace", Workspace.WorkspaceName, Workspace.Note);
        }



        


    }
}

