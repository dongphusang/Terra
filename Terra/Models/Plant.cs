using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terra.Models
{
    public partial class Plant : ObservableObject
    {
        [ObservableProperty]
        public int soilMoisture;
        [ObservableProperty]
        public int temperature;
        [ObservableProperty]
        public int humidity;
        [ObservableProperty]
        public int waterLevel;
        [ObservableProperty]
        public int light;
        [ObservableProperty]
        public string name;
        [ObservableProperty]
        public string note;
    }
}
