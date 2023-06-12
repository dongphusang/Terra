using System;
using Newtonsoft.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

using Terra.Models;
using Terra.Services;

using SkiaSharp;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.Measure;
using LiveChartsCore.Defaults;


namespace Terra.ViewModels
{
	public class GraphicalPlantViewModel : ObservableObject
	{
		private Plant _plant; // model

		// graphical attributes
		private ObservableValue _waterLevel;
		private ObservableValue _light;
		private ObservableValue _temperature;
		private ObservableValue _soilMoisture;
		private ObservableValue _humidity;

		// service objects
		private WorkspaceService _workspaceService;
		private InfluxService _influxService;

		public IEnumerable<ISeries> Series { get; set; } // graphical series

        // constructor
        public GraphicalPlantViewModel()
		{
            InitModelAndService(); // create service and model object
            InitGraphAttributes(); // graph attributes init
            InitSeries();          // init and draw graphs
        }

        /// <summary>
        /// Invoke influx service object to query data from InfluxDB. Discard broken frames.
        /// Then break down data and assign them to Plant model
        /// </summary>
        public void GetDataFromInflux()
        {
            // get data frame from Influx
            var data = Unwrap(Task.Run(() => _influxService.GetData()));

            // check if data frame is corrupted (a normal frame has five attributes. A broken frame has 10 attributes)
            if (data.Split(",").Length == 5)
            {
                _plant = JsonConvert.DeserializeObject<Plant>(data); // break down data

                // dynamically update graph attributes
                _waterLevel.Value   = _plant.WaterLevel;               
                _light.Value        = _plant.Light;
                _temperature.Value  = _plant.Temperature;
                _humidity.Value     = _plant.Humidity;
                _soilMoisture.Value = _plant.SoilMoisture;
            }
            else
            {
                _waterLevel.Value = 0;
                _light.Value = 0;
                _temperature.Value = 0;
                _humidity.Value = 0;
                _soilMoisture.Value = 0;
            }
        }

        /// <summary>
        /// Check if Task<> object is null. Used for parsing sqlite query results.
        /// </summary>
        /// <param name="obj"> sqlite query result </param>
        /// <returns> Parsed value of sqlite query result. </returns>
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

        /// <summary>
        /// Init graph attributes.
        /// </summary>
        private void InitGraphAttributes()
        {
            _waterLevel = new() { Value = 0 };
            _light = new() { Value = 0 };
            _temperature = new() { Value = 0 };
            _soilMoisture = new() { Value = 0 };
            _humidity = new() { Value = 0 };
        }

        /// <summary>
        /// Init and draw graphs based on defined attributes.
        /// </summary>
        private void InitSeries()
        {
            Series = new GaugeBuilder()
                .WithLabelsSize(50)
                .WithInnerRadius(75)
                .WithBackgroundInnerRadius(75)
                .WithBackground(new SolidColorPaint(new SKColor(100, 181, 246, 90)))
                .WithLabelsPosition(PolarLabelsPosition.ChartCenter)
                .AddValue(_waterLevel)
                .BuildSeries();
        }

        /// <summary>
        /// Init model and service objects.
        /// </summary>
        private void InitModelAndService()
        {
            _plant = new();
            _workspaceService = new();
            _influxService = new();
        }
    }
}

