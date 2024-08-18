namespace MiHotkeys.Services.MiDevice.Commands;

public class ResumeChargingProtectCommand : IMiCommand
{
    public string Method { get; }
    public object Params { get; }

    public ResumeChargingProtectCommand()
    {
        Method = "resume_charging_protect";
        Params = new object();
    }
}