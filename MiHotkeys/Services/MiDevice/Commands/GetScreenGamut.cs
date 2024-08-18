namespace MiHotkeys.Services.MiDevice.Commands;

public class GetScreenGamut : IMiCommand
{
    public string  Method => CommandsList.GetScreenGamut;
    public object? Params => null;
}