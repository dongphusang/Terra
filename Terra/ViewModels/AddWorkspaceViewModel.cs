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
        private WorkspaceService workspaceService;

        public AddWorkspaceViewModel()
        {
            Workspace = new();
            workspaceService= new WorkspaceService();
        }

        [RelayCommand]
        void PostWorkspace()
        {
             workspaceService.InsertToTable("Workspace", Workspace.WorkspaceName, Workspace.Note);
        }



        


    }
}

