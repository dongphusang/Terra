using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terra.Models
{
    public partial class TerraEmail : ObservableObject
    {
        [ObservableProperty]
        public string email;
    }
}
