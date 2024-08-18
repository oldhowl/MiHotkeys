namespace MiHotkeys.Services.MiDevice.Commands;

public class GetTurboEngineEnableQuery : IMiCommand
{
    public string  Method => CommandsList.GetTurboEngineEnable;
    public object? Params => null;
}