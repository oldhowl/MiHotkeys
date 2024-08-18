namespace MiHotkeys.Services.MiDevice.Commands;

public class RequestDeviceLogQuery : IMiCommand
{
    public string  Method => CommandsList.RequestDeviceLog;
    public object? Params { get; }= null;
}