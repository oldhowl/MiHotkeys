namespace MiHotkeys.Services.MiDevice;

public interface IMiCommand
{
    public string  Method { get; }
    public object? Params { get; }
    
}