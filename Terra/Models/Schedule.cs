using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terra.Models
{
    partial class Schedule : ObservableObject
    {
        [ObservableProperty]
        public string weekDay;
        [ObservableProperty]
        public TimeSpan time;
    }
}
