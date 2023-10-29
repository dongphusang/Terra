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
        public Plant plantModel; // plant model
        [ObservableProperty]
        public APIPlant apiPlantModel;

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
            PlantModel = new();
            ApiPlantModel = new();

            _workspaceService = new();
            _influxService = new();
            _plantAPIService = new();

            ScreenHeight = DeviceDisplay.MainDisplayInfo.Height;
            ScreenWidth = DeviceDisplay.MainDisplayInfo.Width;
            isAdditionRestricted = true;

            PlantNames = new();
            WarningIcons = new();

            CurrentWorkspaceName = Preferences.Get("CurrentWorkspace", string.Empty); // get value from preferences (which assigned in WorkspaceViewModel)
            CurrentPlantName = Unwrap(Task.Run(() => _workspaceService.GetPlantName(CurrentWorkspaceName)));           
        }

        // navigate to specific plant display
        [RelayCommand]
        async Task ToWorkspaceDisplay() => await Task.Run(() => Shell.Current.GoToAsync(nameof(AddPlantPage), false));

        // navigate to graphical view
        [RelayCommand]
        async Task ToGraphicalPage() => await Task.Run(() => Shell.Current.GoToAsync(nameof(GraphicalView), false));

        // navigate to plant info page
        [RelayCommand]
        async Task ToPlantInfoPage() => await Task.Run(() => Shell.Current.GoToAsync(nameof(PlantInfoPage), false));

        // navigate to SubToPlantPage
        [RelayCommand]
        async Task ToPlantSubscribing() => await Task.Run(() => Shell.Current.GoToAsync(nameof(SubToPlantPage), false));

        [RelayCommand]
        async Task ToOperatingConfig() => await Task.Run(() => Shell.Current.GoToAsync(nameof(ModeConfigPage), false));


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
            if (PlantModel.Name is null)
            {
                // make toast: want users to choose a plant from list of plant names
                ThrowToast("Choose Plant!");
                // don't update database
                return Task.CompletedTask;
            }
            // insert plant entry to sqlite
            _workspaceService.InsertToPlantTable(CurrentWorkspaceName, PlantModel.Name, PlantModel.Note);
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
            ApiPlantModel = _plantAPIService.GetPlantDetails(id).Result;
            ApiPlantModel.Light = string.Join(", ", ApiPlantModel.Sunlight);
            ApiPlantModel.PropagationMethods = string.Join(", ", ApiPlantModel.Propagation);
            PlantModel.Note = Unwrap(Task.Run(() => _workspaceService.GetPlantNote(CurrentPlantName)));
        }

        /// <summary>
        /// Invoke influx service object to query data from InfluxDB, discard broken frames,
        /// then break down data and assign them to Plant model.
        /// </summary>
        public async Task GetDataFromInflux()
        {
            // get MCU associated with current workspace
            var targetMCU = Unwrap(Task.Run(() => _workspaceService.GetWorkspaceMCU(CurrentWorkspaceName)));
            // get data frame from Influx
            var data = await _influxService.GetData(targetMCU);
            // check if data frame is corrupted (a normal frame has five attributes. A broken frame has 10 attributes)
            if (data.Split(",").Length == 5)
            {
                PlantModel = JsonConvert.DeserializeObject<Plant>(data); // break down data
            }
            else
            {
                PlantModel.SoilMoisture = 0;
                PlantModel.Light = 0;
                PlantModel.Temperature = 0;
                PlantModel.Humidity = 0;
                PlantModel.WaterLevel = 0;
            }
            Console.WriteLine($"SANG: "+PlantModel.SoilMoisture);
        }

        // assess current data and give corresponding warnings
        public void AssessWarnings()
        {
            WarningIcons = new();
            if (PlantModel.SoilMoisture < 300)
            {
                WarningIcons.Add("moisture_warning.svg");
            }
            if (PlantModel.Light < 100)
            {
                WarningIcons.Add("light_warning.svg");
            }
            if (PlantModel.Temperature < 23)
            {
                WarningIcons.Add("temp_warning.svg");
            }
            if (PlantModel.Humidity > 60)
            {
                WarningIcons.Add("humidity_warning.svg");
            }
            if (PlantModel.WaterLevel < 10)
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

        /// <summary>
        /// Check if Task<> object is null. Used for parsing InfluxDB query results.
        /// </summary>
        /// <param name="obj"> InfluxDB query result </param>
        /// <returns> Parsed value of InfluxDB query result. </returns>
        private string Unwrap(Task<string> obj)
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

