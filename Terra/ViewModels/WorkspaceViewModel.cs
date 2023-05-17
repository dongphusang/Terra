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
        // list of workspace names and notes
        [ObservableProperty]
        public List<string> workspaceNameAndNote;

        // workspace service object
        private WorkspaceService _workspaceService;       

        // constructor
        public WorkspaceViewModel()
        {
            Workspace = new();
            _workspaceService= new WorkspaceService();
        }

        // posting new workspace entry onto Terra database
        [RelayCommand]
        Task PostWorkspace()
        {
             _workspaceService.InsertToTable("Workspace", Workspace.WorkspaceName, Workspace.Note);
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

        // manually update labels upon navigation to a new page
        [RelayCommand]
        public void UpdateWorkspace()
        {
            WorkspaceNameAndNote = _workspaceService.GetWorkspaces("Workspace")
                ?? new List<string>() { "NA", "NA", "NA", "NA", "NA", "NA", "NA", "NA" };
        }

        // delete workspace of choice
        [RelayCommand]
        public void DeleteWorkspace(string name)
        {
            _workspaceService.DeleteWorkspace(name); // delete on db
            UpdateWorkspace(); // delete on UI (removing an element of workspaces List)
        }

        // navigate to workspace
        [RelayCommand]
        Task ToWorkspace()
        {
            return Shell.Current.GoToAsync(nameof(WorkspaceDisplay));
        }



    }
}

