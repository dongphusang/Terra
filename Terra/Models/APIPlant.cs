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
        [ObservableProperty]
        public string selectedPlant;
        [ObservableProperty]
        public int id;
        [ObservableProperty]
        public string common_name;
        [ObservableProperty]
        public string watering;
        [ObservableProperty]
        public List<string> sunlight;
    }
}
