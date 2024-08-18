namespace MiHotkeys.Services.MiDevice.Commands;

public class RegisterTurboEngineChangedCommand : IMiCommand
{
    public string  Method => CommandsList.RegisterTurboEngineChanged;
    public object? Params { get; }= null;
}