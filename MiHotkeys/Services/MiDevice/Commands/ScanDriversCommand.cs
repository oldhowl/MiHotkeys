namespace MiHotkeys.Services.MiDevice.Commands;

public class ScanDriversCommand : IMiCommand
{
    public string Method => CommandsList.ScanDrivers;
    public object? Params => null;
}