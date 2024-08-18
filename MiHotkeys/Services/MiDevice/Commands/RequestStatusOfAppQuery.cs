namespace MiHotkeys.Services.MiDevice.Commands;

public class RequestStatusOfAppQuery : IMiCommand
{
    public string  Method => CommandsList.RequestStatusOfApp;
    public object? Params { get; }= null;
    
}