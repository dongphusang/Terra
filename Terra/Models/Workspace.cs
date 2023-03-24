using System.Text;

namespace Terra.Models;

public class Workspace
{
    public String WorkspaceName { get; set; } // workspace name
    public String Note { get; set; }          // note on workspace upon creation
    public bool IsLightModule { get; set; }   // does workspace's plants have light automation
    public bool IsWaterModule { get; set; }   // does workspace's plants have water automation
}
