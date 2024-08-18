namespace MiHotkeys.Services.PowerManager;

public interface IPowerModeProvider
{
    void RequestCurrentPowerMode();

    void SetPowerMode(int mode);
}