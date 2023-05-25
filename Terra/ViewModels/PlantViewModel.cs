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
        [ObservableProperty]
        public string currentWorkspaceName; // name of currently chosen workspace (representing a group of plants)

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

