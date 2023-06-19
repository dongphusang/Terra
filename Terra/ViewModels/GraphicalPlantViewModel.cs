using System;
using Newtonsoft.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

using Terra.Models;
using Terra.Services;
using Terra.TerraConstants;

using SkiaSharp;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.Measure;
using System.Collections.ObjectModel;

namespace Terra.ViewModels
{
	public class GraphicalPlantViewModel : ObservableObject
	{
		private Plant _plant; // model

		// service objects
		private WorkspaceService _workspaceService;
		private InfluxService _influxService;

        private ObservableValue _waterLevelVal; // water level for gauge chart
        private ObservableValue _lightVal;      // light level for gauge chart
        private ObservableCollection<ObservableValue> _temperaturePoints;  // line chart points
        private ObservableCollection<ObservableValue> _humidityPoints;     // line chart points
        private ObservableCollection<ObservableValue> _soilMoisturePoints; // line chart points

        public ObservableCollection<ISeries> TempAndHumid { get; set; } // line chart #1: humidity and temperature
        public ObservableCollection<ISeries> SoilMoisture { get; set; } // line chart #2: soil moisture
        public IEnumerable<ISeries> WaterLevelGauge { get; set; }       // gauge chart for water level
        public IEnumerable<ISeries> LightGauge { get; set; }            // gauge chart for light

        public Axis[] XAxis { get; set; }             // attribute to override default x-axis style
        public Axis[] YAxisTempHumid { get; set; }    // attribute to override default y-axis style (temp and humind line chart)
        public Axis[] YAxisSoilMoisture { get; set; } // attribute to override default y-axis style (soil moisture line chart)6 

        // constructor
        public GraphicalPlantViewModel()
		{
            InitModelAndService();   // create service and model object
            InitGraphAttributes();   // graph attributes init
            InitXAxis();             // init x-axis for cartesian charts
            InitYAxisTempHumid();    // init y-axis for temperature and humidity line chart
            InitYAxisSoilMoisture(); // init y-axis for soil moisture line chart
            InitSeries();            // init and draw charts
        }

        /// <summary>
        /// Invoke influx service object to query data from InfluxDB. Discard broken frames.
        /// Then break down data and assign them to Plant model
        /// </summary>
        public string GetDataFromInflux()
        {   
            // get data frame from Influx
            var data = Unwrap(Task.Run(_influxService.GetData));
    
            // check if data frame is corrupted (a normal frame has five attributes. A broken frame has 10 attributes)
            if (data.Split(",").Length == 5)
            {
                _plant = JsonConvert.DeserializeObject<Plant>(data); // break down data

                // dynamically update graph attributes
                _waterLevelVal.Value = _plant.WaterLevel;
                _lightVal.Value = _plant.Light;
                AddDataPoint(_plant.Temperature, _temperaturePoints);
                AddDataPoint(_plant.Humidity, _humidityPoints);
                AddDataPoint(_plant.SoilMoisture, _soilMoisturePoints);
            }
            else
            {
                // assign value of zero to represent corrupted data
                _waterLevelVal.Value = GraphConstants.ZERO;
                _lightVal.Value      = GraphConstants.ZERO;
                AddDataPoint(GraphConstants.ZERO, _temperaturePoints);
                AddDataPoint(GraphConstants.ZERO, _humidityPoints);
                AddDataPoint(GraphConstants.ZERO, _soilMoisturePoints);
            }

            return data;
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
                return GraphConstants.NOT_APPLICABLE;
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
                return GraphConstants.NOT_APPLICABLE;
            }
            return result.ToString();
        }

        /// <summary>
        /// Init graph attributes.
        /// </summary>
        private void InitGraphAttributes()
        {
            _waterLevelVal      = new() { Value = GraphConstants.ZERO };
            _lightVal           = new() { Value = GraphConstants.ZERO };
            _temperaturePoints  = new();
            _humidityPoints     = new();
            _soilMoisturePoints = new();
        }

        /// <summary>
        /// Init and draw graphs based on defined attributes.
        /// </summary>
        private void InitSeries()
        {
            // line chart for temperature and humidity
            TempAndHumid = new()
            {
                new LineSeries<ObservableValue>
                {
                    Name = GraphConstants.HUMIDITY_CHART_LABEL,
                    Values = _humidityPoints,
                    Fill = null,
                },
                new LineSeries<ObservableValue>
                {
                    Name = GraphConstants.TEMPERATURE_CHART_LABEL,
                    Values = _temperaturePoints,
                    Fill = null,
                },               
            };

            // line chart for soil moisture
            SoilMoisture = new()
            {
                new LineSeries<ObservableValue>
                {
                    Name = GraphConstants.SOILMOISTURE_CHART_LABEL,
                    Values = _soilMoisturePoints,
                    Fill = null,
                }
            };

            // gauge chart for light level
            LightGauge = new GaugeBuilder()
                .WithLabelsSize(50)
                .WithInnerRadius(75)
                .WithBackgroundInnerRadius(75)
                .WithBackground(new SolidColorPaint(new SKColor(255, 247, 219, 90)))
                .WithLabelsPosition(PolarLabelsPosition.ChartCenter)
                .AddValue(_lightVal, GraphConstants.LIGHT_GAUGE_LABEL, new SKColor(255, 220, 95, 90), SKColors.Red) // defines the value and the color 
                .BuildSeries();

            // gauge chart for water level
            WaterLevelGauge = new GaugeBuilder()
                .WithLabelsSize(50)
                .WithInnerRadius(75)
                .WithBackgroundInnerRadius(75)
                .WithBackground(new SolidColorPaint(new SKColor(183, 207, 255, 90)))
                .WithLabelsPosition(PolarLabelsPosition.ChartCenter)
                .AddValue(_waterLevelVal, GraphConstants.WATERLEVEL_GAUGE_LABEL, new SKColor(44, 115, 255, 90), SKColors.Red) // defines the value and the color 
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

        /// <summary>
        /// main: Add data point to line chart | explaination: Every line chart can show seven points at a time. With
        /// every extra point, we remove the oldest point (first point) in the chart, offset all points' position
        /// by minus one, then add the newest point
        /// at the end of the chart.
        /// </summary>
        /// <param name="point"> Data point to be added. </param>
        /// <param name="chartPoints"> Target line chart. </param>
        private void AddDataPoint(int point, ObservableCollection<ObservableValue> chartPoints)
        {
            if (chartPoints.Count is not GraphConstants.MAXED_POINT_CAPACITY)
            {
                chartPoints.Add(new ObservableValue(point));
            }
            else
            {
                chartPoints.RemoveAt(0);
                chartPoints.Add(new ObservableValue(point));
            }
        }

        /// <summary>
        /// Init custom x-axis for line charts.
        /// </summary>
        private void InitXAxis()
        {
            XAxis = new Axis[]
            {
                new Axis
                {
                    Name = GraphConstants.X_AXIS_LABEL,

                    LabelsPaint = new SolidColorPaint(SKColors.Blue),
                    TextSize = 10,
                }
            };
            
        }

        /// <summary>
        /// Init custom y-axis for line chart (temperature and humidity)
        /// </summary>
        private void InitYAxisTempHumid()
        {
            YAxisTempHumid = new Axis[]
            {
                new Axis
                {
                    Name = GraphConstants.Y_AXIS_TEMPxHUMID_LABEL, 

                    LabelsPaint = new SolidColorPaint(SKColors.Blue),
                    LabelsRotation = 90,
                    TextSize = 10,                   
                }
            };
        }

        /// <summary>
        /// Init custom y-axis for line chart (soil moisture)
        /// </summary>
        private void InitYAxisSoilMoisture()
        {
            YAxisSoilMoisture = new Axis[]
            {
                new Axis
                {
                    Name = GraphConstants.Y_AXIS_SOILMOISTURE_LABEL,

                    LabelsPaint = new SolidColorPaint(SKColors.Blue),
                    LabelsRotation = 90,
                    TextSize = 10,
                }
            };
        }

    }
}

