namespace MiHotkeys.Services.MiDevice.Commands;

public class SetTurboEngineCommand : IMiCommand
{
    public string  Method => CommandsList.SetTurboEngine;
    public object? Params { get; }

    public SetTurboEngineCommand(bool enable)
    {
        Params = new Dictionary<string, object>()
        {
            ["on_off"] = enable ? 1 : 0
        };
    }
}