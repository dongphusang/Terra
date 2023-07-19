// separate this class into numerical(current) and GraphicalPlantViewModel?

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
	public partial class PlantViewModel : ObservableObject
	{
        [ObservableProperty]
        public Plant plant; // plant model

        public string CurrentWorkspaceName { get; set; } // name of currently chosen workspace (representing a group of plants)
        public string CurrentPlantName { get; set; } // name of currently chosen plant

        // service objects
        private WorkspaceService _workspaceService;
        private InfluxService _influxService;

        // constructor
        public PlantViewModel()
        {
            Plant = new();
            _workspaceService = new();
            _influxService = new();

            CurrentWorkspaceName = Preferences.Get("CurrentWorkspace", string.Empty); // get value from preferences (which assigned in WorkspaceViewModel)
            CurrentPlantName = Unwrap(Task.Run(() => _workspaceService.GetPlantName(CurrentWorkspaceName)));           
        }

        // navigate to specific plant display
        [RelayCommand]
        Task ToWorkspaceDisplay() => Shell.Current.GoToAsync(nameof(AddPlantPage));

        // navigate to graphical view
        [RelayCommand]
        Task ToGraphicalPage() => Shell.Current.GoToAsync(nameof(GraphicalView));

        // navigate to plant info page
        [RelayCommand]
        Task ToPlantInfoPage() => Shell.Current.GoToAsync(nameof(PlantInfoPage));

        /// <summary>
        /// Add plant entry to database. (Not able to add to Plant table)
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public Task PostPlant()
        {
            // insert plant entry to sqlite
            _workspaceService.InsertToPlantTable(CurrentWorkspaceName, Plant.Name, Plant.Note);
            // make a toast
            {
                var message = "Plant added!";
                ToastDuration duration = ToastDuration.Short;
                var fontSize = 14;
                Toast.Make(message, duration, fontSize).Show();
            }
            // navigate to WorkspaceDisplay
            return Shell.Current.GoToAsync("///MainPage");
        }

        /// <summary>
        /// Invoke influx service object to query data from InfluxDB. Discard broken frames.
        /// Then break down data and assign them to Plant model
        /// </summary>
        public void GetDataFromInflux()
        {
            // get data frame from Influx
            var data = Task.Run(() => _influxService.GetData()).Result;
            // check if data frame is corrupted (a normal frame has five attributes. A broken frame has 10 attributes)
            if (data.Split(",").Length == 5)
            {
                Plant = JsonConvert.DeserializeObject<Plant>(data); // break down data
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

        // check if object returned from Task.Run() is null. Return non-null value. Usually used for sqlite operations
        private string Unwrap(Task<object> obj)
        {
            var result = obj.Result;
            if (result is null)
            {
                return "N/A";
            }
            return result.ToString();
        }


    }
}

