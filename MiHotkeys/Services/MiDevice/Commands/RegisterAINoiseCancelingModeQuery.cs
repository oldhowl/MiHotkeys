namespace MiHotkeys.Services.MiDevice.Commands;

public class RegisterAINoiseCancelingModeQuery : IMiCommand
{
    public string  Method => CommandsList.RegisterAiNoiseCancelingMod;
    public object? Params { get; }
}