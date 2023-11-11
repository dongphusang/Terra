/// <summary>
/// This class exclusively provides constants to GraphicalPlantViewModel
/// </summary>

using System;
namespace Terra.TerraConstants
{
	public class GraphConstants
	{
        public const string NOT_APPLICABLE  = "N/A";

        public const string TEMPERATURE_CHART_LABEL = "Temp (C)";

        public const string HUMIDITY_CHART_LABEL = "Humid (%)";

        public const string SOILMOISTURE_CHART_LABEL = "SM";

        public const string LIGHT_GAUGE_LABEL = "Light (%)";

        public const string WATERLEVEL_GAUGE_LABEL = "Water Tank (%)";

        public const string X_AXIS_LABEL = "Time";

        public const string Y_AXIS_TEMPxHUMID_LABEL = "(Temp: C | Humid: %)";

        public const string Y_AXIS_SOILMOISTURE_LABEL = "Soil Moisture";

        public const int ZERO = 0; 

        public const int MAXED_POINT_CAPACITY_SHORT = 7; // line chart's max point capacity; seven points are displayable at any time

        public const int MAXED_POINT_CAPACITY_LONG = 21; // line chart's max point capacity; 15 points are displayable at any time
    }
}

