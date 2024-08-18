namespace MiHotkeys.Services.MiDevice.Commands;

public class GetWorkloadMode : IMiCommand
{
    public string  Method => "get_workLoad_mode";
    public object? Params { get; }= null;
}