using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Maui.Alerts;
using Terra.Models;
using Terra.Services;
using CommunityToolkit.Maui.Core;

namespace Terra.ViewModels
{
    public partial class WorkspaceViewModel : ObservableObject
    {
        [ObservableProperty]
        public Workspace workspace;  
        private WorkspaceService workspaceService;

        public WorkspaceViewModel()
        {
            Workspace = new();
            workspaceService= new WorkspaceService();
        }

        [RelayCommand]
        Task PostWorkspace()
        {
             workspaceService.InsertToTable("Workspace", Workspace.WorkspaceName, Workspace.Note);
            // make a toast
            {
                var message = "Workspace added!";
                ToastDuration duration = ToastDuration.Short;
                var fontSize = 14;
                Toast.Make(message, duration, fontSize).Show();             
            }
            // navigate to Main
            return Shell.Current.GoToAsync("///MainPage");
        }





        


    }
}

