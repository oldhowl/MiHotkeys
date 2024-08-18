namespace MiHotkeys.Services.MiDevice.Commands;

public class SetPerfomanceModeCommand : IMiCommand
{
    public string  Method => CommandsList.SetPerformanceMode;
    public object? Params { get; }

    public SetPerfomanceModeCommand(bool isEnabled)
    {
        Params = new Dictionary<string, object>()
        {
            ["mode"] = isEnabled ? 1 : 0
        };
    }
}