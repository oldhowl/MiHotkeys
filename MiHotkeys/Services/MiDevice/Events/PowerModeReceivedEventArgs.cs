namespace MiHotkeys.Services.MiDevice.Events;

public class PowerModeReceivedEventArgs : EventArgs
{
    public int Mode { get; }

    public PowerModeReceivedEventArgs(int mode)
    {
        Mode = mode;
    }
}