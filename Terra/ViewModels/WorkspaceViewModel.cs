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
        public Workspace workspaceModel; // workspace model
        [ObservableProperty]
        public List<string> workspaceNameAndNote; // list of names and notes
        [ObservableProperty]
        public bool isAdditionRestricted; // is workspace addition disabled
        [ObservableProperty]
        public string restrictionMessage; // popup message when restricted addition
        [ObservableProperty]
        public List<string> measurements; // measurement representing a MCU, to pull data from 

        // service objects
        private WorkspaceService _workspaceService;
        private InfluxService _influxService; 


        // constructor
        public WorkspaceViewModel()
        {
            WorkspaceModel = new();
            _influxService = new();
            _workspaceService = new();
            isAdditionRestricted = EvalAddibility();
            Measurements = new List<string>(Task.Run(_influxService.RetrieveMeasurements).Result);
        }

        // call service method to see if user is allowed to add more workspaces
        public bool EvalAddibility()
        {
            var result = Unwrap(Task.Run(_workspaceService.GetNumberofWorkspaces));

            if (int.Parse(result) is -1 or 0) // no workspace currently, allow to add
            {
                RestrictionMessage = string.Empty;
                return true;
            }
            // there is workspace, not allowed to add more
            RestrictionMessage = "*Addibility reached limit, unable to add workspace";
            return false;                     
        }

        // posting new workspace entry onto Terra database
        [RelayCommand]
        Task PostWorkspace()
        {
            if (WorkspaceModel.WorkspaceName is not null && WorkspaceModel.WorkspaceName.Trim().Equals("") is false && WorkspaceModel.Microcontroller is not null) {
                // insert into sqlite table
                _workspaceService.InsertToWorkspaceTable(WorkspaceModel.WorkspaceName.Trim(), WorkspaceModel.Microcontroller, WorkspaceModel.Note is null ? "None" : WorkspaceModel.Note.Trim());

                // make a toast
                ThrowToast("Workspace added!");

                // navigate to Main
                return Shell.Current.GoToAsync("///MainPage");
            }
            else
            {
                ThrowToast("Field left empty!");

                return Task.CompletedTask;
            }
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
        public async Task ToWorkspace(string workspaceName)
        {
            if (workspaceName is null) // make a toast
            {
                ThrowToast("Please Add Your Workspace");
            }
            else
            {
                Preferences.Set("CurrentWorkspace", workspaceName);

                var columnCount = Convert.ToInt32(await _workspaceService.CountColumnValues(workspaceName));

                if (columnCount is not 0)
                    await Shell.Current.GoToAsync(nameof(WorkspaceDisplay), false);  
                else
                    await Shell.Current.GoToAsync(nameof(EmptyPlantSlot), false);               
            }
        }

        // check if object returned from Task.Run() is null. Return non-null value
        private string Unwrap(Task<object> obj)
        {
            var result = obj.Result;
            if (result is null || result is 0)
            {
                return "-1";
            }
            return result.ToString();
        }

        // throw toast of a message
        private void ThrowToast(string message)
        {
            ToastDuration duration = ToastDuration.Short;
            var fontSize = 14;
            Toast.Make(message, duration, fontSize).Show();
        }
    }
}

