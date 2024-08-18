namespace MiHotkeys.Services.MiDevice.Commands;

public class GetWorkLoadModeDecepticonEn : IMiCommand
{
    public string  Method => CommandsList.GetWorkLoadModeDecepticonEn;
    public object? Params { get; }= null;
}