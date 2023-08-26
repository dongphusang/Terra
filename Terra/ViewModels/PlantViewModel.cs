// separate this class into numerical(current) and GraphicalPlantViewModel?

using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Maui.Alerts;
using Terra.Models;
using Terra.Services;
using CommunityToolkit.Maui.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace Terra.ViewModels
{
	public partial class PlantViewModel : ObservableObject
	{
        [ObservableProperty]
        public Plant plant; // plant model
        [ObservableProperty]
        public APIPlant apiPlant;

        public string CurrentWorkspaceName { get; set; } // name of currently chosen workspace (representing a group of plants)
        public string CurrentPlantName { get; set; } // name of currently chosen plant

        // service objects
        private WorkspaceService _workspaceService;
        private InfluxService _influxService;
        private PlantAPIService _plantAPIService;

        // warnings
        [ObservableProperty]
        private bool isAdditionRestricted; // plant addition warning
        [ObservableProperty]
        List<string> warningIcons; // plant attribute warning

        [ObservableProperty]
        double screenHeight;
        [ObservableProperty]
        double screenWidth;
        [ObservableProperty]
        List<string> plantNames; // retrieved from api

        

        // constructor
        public PlantViewModel()
        {
            Plant = new();
            ApiPlant = new();

            _workspaceService = new();
            _influxService = new();
            _plantAPIService = new();

            ScreenHeight = DeviceDisplay.MainDisplayInfo.Height;
            ScreenWidth = DeviceDisplay.MainDisplayInfo.Width;
            isAdditionRestricted = true;

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

        // navigate to SubToPlantPage
        [RelayCommand]
        Task ToPlantSubscribing() => Shell.Current.GoToAsync(nameof(SubToPlantPage));

        /// <summary>
        /// Update collection view that displays matching plants with user's entry.
        /// </summary>
        /// <param name="query"> Entry's text characters. </param>
        [RelayCommand]
        public async Task UpdateCollectionView(string query)
        {
            PlantNames = await _plantAPIService.GetPlantOptions(query);
        }

        /// <summary>
        /// Add plant entry to database.
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public Task PostPlant()
        {
            if (Plant.Name is null)
            {
                // make toast: want users to choose a plant from list of plant names
                ThrowToast("Choose Plant!");
                // don't update database
                return Task.CompletedTask;
            }
            // insert plant entry to sqlite
            _workspaceService.InsertToPlantTable(CurrentWorkspaceName, Plant.Name, Plant.Note);
            // make  toast: notify users the plant has been added
            ThrowToast("Plant Added!");

            // navigate to WorkspaceDisplay
            return Shell.Current.GoToAsync("///MainPage");
        }

        /// <summary>
        /// 
        /// </summary>
        public void GetPlantDataFromAPI()
        {
            var id = _plantAPIService.GetPlantID(CurrentPlantName).Result;
            ApiPlant = _plantAPIService.GetPlantDetails(id).Result;
            ApiPlant.Light = string.Join(", ", ApiPlant.Sunlight);
            ApiPlant.PropagationMethods = string.Join(", ", ApiPlant.Propagation);
            Plant.Note = Unwrap(Task.Run(() => _workspaceService.GetPlantNote(CurrentPlantName)));
        }

        /// <summary>
        /// Invoke influx service object to query data from InfluxDB, discard broken frames,
        /// then break down data and assign them to Plant model.
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

        // assess current data and give corresponding warnings
        public void AssessWarnings()
        {
            WarningIcons = new();
            if (Plant.SoilMoisture < 300)
            {
                WarningIcons.Add("moisture_warning.svg");
            }
            if (Plant.Light < 100)
            {
                WarningIcons.Add("light_warning.svg");
            }
            if (Plant.Temperature < 23)
            {
                WarningIcons.Add("temp_warning.svg");
            }
            if (Plant.Humidity > 60)
            {
                WarningIcons.Add("humidity_warning.svg");
            }
            if (Plant.WaterLevel < 10)
            {
                WarningIcons.Add("water_tank_warning.svg");
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

        // throw toast of a message
        private void ThrowToast(string message)
        {
            ToastDuration duration = ToastDuration.Short;
            var fontSize = 14;
            Toast.Make(message, duration, fontSize).Show();
        }


    }
}

