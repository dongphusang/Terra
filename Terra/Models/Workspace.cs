using System.Text;

namespace Terra.Models;

public class Workspace
{
    public String WorkspaceName { get; set; }
    public String Note { get; set; }
    public bool IsLightingExist { get; set; }
    public bool IsWateringExist { get; set; }
}
