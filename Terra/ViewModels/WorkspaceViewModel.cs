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
        // workspace model object
        [ObservableProperty]
        public Workspace workspace;  
        private WorkspaceService workspaceService;

        // workspace names and notes
        [ObservableProperty]
        private string firstWpName;
        [ObservableProperty]
        private string secondWpName;
        [ObservableProperty]
        private string thirdWpName;
        [ObservableProperty]
        private string fourthWpName;
        [ObservableProperty]
        private string firstWpNote;
        [ObservableProperty]
        private string secondWpNote;
        [ObservableProperty]
        private string thirdWpNote;
        [ObservableProperty]
        private string fourthWpNote;


        public WorkspaceViewModel()
        {
            Workspace = new();
            workspaceService= new WorkspaceService();
        }

        // posting new workspace entry onto Terra database
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


        public void PullWorkspaces()
        {
            // call a method in workspaceService to retrieve available workspaces, including names and notes
            // in xml file, bind note and name variables in this class to approrpiate candidate
        }





        


    }
}

