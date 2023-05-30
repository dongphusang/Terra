using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Maui.Alerts;
using Terra.Models;
using Terra.Services;
using CommunityToolkit.Maui.Core;
using Newtonsoft.Json;

namespace Terra.ViewModels
{
    public partial class WorkspaceViewModel : ObservableObject
    {
        [ObservableProperty]
        public Workspace workspace; // workspace model
        [ObservableProperty]
        public List<string> workspaceNameAndNote; // list of names and notes

        // service objects
        private WorkspaceService _workspaceService;
        private InfluxService _influxService; 


        // constructor
        public WorkspaceViewModel()
        {
            Workspace = new();
            _influxService = new();
            _workspaceService = new();
        }


        // posting new workspace entry onto Terra database
        [RelayCommand]
        Task PostWorkspace()
        {
             _workspaceService.InsertToWorkspaceTable(Workspace.WorkspaceName, Workspace.Note);
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

        // navigate to workspace and assign current workspace to mem
        [RelayCommand]
        public void ToWorkspace(string workspaceName)
        {
            if (workspaceName is null) // make a toast
            {
                var message = "Please Add Your Workspace";
                ToastDuration duration = ToastDuration.Short;
                var fontSize = 14;
                Toast.Make(message, duration, fontSize).Show();
            }
            else
            {
                if (Preferences.ContainsKey("CurrentWorkspace") is false)
                    Preferences.Set("CurrentWorkspace", workspaceName);

                var columnCount = Convert.ToInt32(Task.Run(() => _workspaceService.CountColumnValues(workspaceName)).Result);

                Console.WriteLine($"ToWorkspace(): {columnCount}");

                if (columnCount is not 0)
                    Shell.Current.GoToAsync(nameof(WorkspaceDisplay));  
                else
                    Shell.Current.GoToAsync(nameof(EmptyPlantSlot));               
            }
        } 
    }
}

