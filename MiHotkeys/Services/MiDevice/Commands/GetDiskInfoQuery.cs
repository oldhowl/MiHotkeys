namespace MiHotkeys.Services.MiDevice.Commands;

public class GetDiskInfoQuery : IMiCommand
{
    public string  Method => CommandsList.GetDiskInfoSc;
    public object? Params { get; }= null;
}