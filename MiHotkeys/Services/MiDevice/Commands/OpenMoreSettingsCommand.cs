namespace MiHotkeys.Services.MiDevice.Commands;

public class OpenMoreSettingsCommand : IMiCommand
{
    public string  Method => CommandsList.OpenMoreSetting;
    public object? Params { get; }= null;
}