using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terra.Models
{
    public partial class APIPlant : ObservableObject
    {
        public int Id { get; set; }
        public string Common_name { get; set; }
        public string Watering { get; set; }    
        public List<string> Sunlight { get; set; } // to be deserialized into, not binded
        public string Growth_rate { get; set; }
        public List<string> Propagation { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// Parsed after Sunlight is deserialized. Because of formatting purposes, this var is parsed using string.Join on Sunlight property to add "," 
        /// to every element and displayed as a single string. Set as [ObservableProperty] to counter the delay in updating UI.
        /// To be binded with light content in PlantInfoPage.xml
        /// </summary>
        [ObservableProperty]
        public string light;

        /// <summary>
        /// Parsed after Propagation is deserialized. Because of formatting purposes, this var is parsed using string.Join on 
        /// Propagation property, adding ", " to every element and displayed as a single string. Set as [ObservableProperty] to 
        /// counter the delay in updating UI. To be binded with propagation content in PlantInfoPage.xml
        /// </summary>
        [ObservableProperty]
        public string propagationMethods;
    }
}
