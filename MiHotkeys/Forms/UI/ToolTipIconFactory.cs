using MiHotkeys.Common;
using MiHotkeys.Services.PowerManager;

namespace MiHotkeys.Forms.UI;

public class ToolTipIconFactory
{
    public static Icon GetIconByState(PowerMode state)
    {
        Func<string, string> buildPath = icon => Path.Combine(ResourcesConstants.ResourcesPath, icon);
        return new Icon(state switch
        {
            PowerMode.Silence  => buildPath(ResourcesConstants.LowIcoFileName),
            PowerMode.Balance  => buildPath(ResourcesConstants.MidIcoFileName),
            PowerMode.MaxPower => buildPath(ResourcesConstants.HighIcoFileName),
            PowerMode.Turbo    => buildPath(ResourcesConstants.FireIcoFileName),
            PowerMode.Pending  => buildPath(ResourcesConstants.RequestIcoFileName),
            _                  => throw new ArgumentOutOfRangeException(nameof(state), state, null)
        });
    }
}