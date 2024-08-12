using System.ComponentModel.Design;
using MiHotkeys.Services.DisplayManager;
using MiHotkeys.Services.PowerManager;

namespace MiHotkeys.Services;

public class CurrentStatuses
{
    public PowerMode       PowerMode       { get; set; }
    public RefreshRateMode RefreshRateMode { get; set; }
}