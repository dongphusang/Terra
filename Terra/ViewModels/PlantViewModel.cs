using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Maui.Alerts;
using Terra.Models;
using Terra.Services;
using CommunityToolkit.Maui.Core;
using Newtonsoft.Json;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace Terra.ViewModels
{
	public partial class PlantViewModel : ObservableObject
	{
        [ObservableProperty]
        public Plant plant; // plant model
        [ObservableProperty]
        public string currentWorkspaceName; // name of currently chosen workspace (representing a group of plants)
        [ObservableProperty]
        public string currentPlantName;
        public ISeries[] Series { get; set; }
        = new ISeries[]
        {
            new LineSeries<double>
            {
                Values = new double[] { 2, 1, 3, 5, 3, 4, 6 },
                Fill = null
            }
        };


        // services objects
        private WorkspaceService _workspaceService;
        private InfluxService _influxService;

        // constructor
        public PlantViewModel()
		{
            Plant = new();
            _workspaceService = new();
            _influxService = new();

            CurrentWorkspaceName = Preferences.Get("CurrentWorkspace", string.Empty); // get value from preferences (which assigned in WorkspaceViewModel)
            CurrentPlantName = Unwrap();
        }

        // navigate to specific plant display
        [RelayCommand]
        Task ToWorkspaceDisplay() => Shell.Current.GoToAsync(nameof(AddPlantPage));

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
        /// </summary>
        public void GetDataFromInflux()
        {
            // get data frame from Influx
            var data = Task.Run(() => _influxService.GetData()).Result;
            // check if data frame is corrupted (a normal frame has five attributes. A broken frame has 10 attributes
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

        // check if object returned from Task.Run() is null. Return non-null value
        private string Unwrap()
        {
            var result = Task.Run(() => _workspaceService.GetPlantName(CurrentWorkspaceName)).Result;
            if (result is null)
            {
                return "N/A";
            }
            return result.ToString();
        }


    }
}

