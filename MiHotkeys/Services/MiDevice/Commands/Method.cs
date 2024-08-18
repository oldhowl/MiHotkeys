namespace MiHotkeys.Services.MiDevice.Commands;

public class MethodQuery : IMiCommand
{
    public string  Method => CommandsList.Method;
    public object? Params { get; }= null;
}