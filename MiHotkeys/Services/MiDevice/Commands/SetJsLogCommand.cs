namespace MiHotkeys.Services.MiDevice.Commands;

public class SetJsLogCommand : IMiCommand
{
    public string  Method => CommandsList.SetJsLog;
    public object? Params { get; }= null;
}