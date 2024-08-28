using System.Runtime.InteropServices;

namespace MiHotkeys.Services.DisplayManager;

public interface IDisplayModeManager
{
    public RefreshRateMode SetNextRefreshRate();
    

    public RefreshRateMode GetCurrentRefreshRateMode();
}


