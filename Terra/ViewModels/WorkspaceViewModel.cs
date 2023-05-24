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
        public Plant plant; // plant model
        [ObservableProperty]
        public List<string> workspaceNameAndNote; // list of names and notes
        [ObservableProperty]
        public string currentWorkspaceName;// name of chosen workspace
        [ObservableProperty]
        public string currentPlantName;// name of chosen plant
       
        private WorkspaceService _workspaceService; // workspace service object
        private InfluxService _influxService; // InfluxDB service object


        // constructor
        public WorkspaceViewModel()
        {
            Workspace = new();
            Plant = new();
            _influxService = new();
            _workspaceService = new ();
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

        public void GetCurrentWorkspace()
        {
            CurrentWorkspaceName = Preferences.Get("CurrentWorkspace", string.Empty);
        }

        // navigate to workspace and assign current workspace to mem
        [RelayCommand]
        void ToWorkspace(string workspaceName)
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
                if (!(Preferences.ContainsKey("CurrentWorkspace")))
                {
                    Preferences.Set("CurrentWorkspace", workspaceName);
                }
                Shell.Current.GoToAsync(nameof(WorkspaceDisplay));
            }
        }

        /// <summary>
        /// Invoke influx service object to query data from InfluxDB. Discard broken frames.
        /// </summary>
        public void GetDataFromInflux()
        {
            // get data frame from Influx
            var data = Task.Run(() => _influxService.GetData()).Result;
            // check if data frame is corrupted (five plant attributes. If received a 10 attribute frame, that is a duplicated or broken frame
            if (data.Split(",").Length == 5)
            {
                Plant = JsonConvert.DeserializeObject<Plant>(data);
            }
            else
            {
                Plant.SoilMoisture = 0;
                Plant.Light = 0;
                Plant.Temperature = 0;
                Plant.Humidity = 0;
                Plant.WaterLevel = 0;
            }  
        }
    }
}

