
using CommunityToolkit.Mvvm.ComponentModel;
using System.Text;

namespace Terra.Models;

public partial class Workspace : ObservableObject
{
    [ObservableProperty]
    public string workspaceName;// workspace name
    [ObservableProperty]
    public string note;          // note on workspace upon creation
    [ObservableProperty]    
    public string microcontroller;          // associated microcontroller on influxdb
}
