using System.Diagnostics;
using System.Management;

namespace MiHotkeys.Services.BatteryInfo
{
    public class BatteryInfoService
    {
        public           PowerLoad PowerLoad { get; private set; }
        private readonly string    _managementScopeCimv2 = "root\\cimv2";
        private readonly string    _managementScopeWmi   = "root\\wmi";

        public BatteryInfoService()
        {
            PowerLoad = GetPowerLoad();
        }


        public PowerLoad GetPowerLoad()
        {
            var fullChargedCapacity =
                GetWmiValue<uint>(_managementScopeWmi, "BatteryFullChargedCapacity", "FullChargedCapacity");
            var   designedCapacity = GetWmiValue<uint>(_managementScopeWmi, "BatteryStaticData", "DesignedCapacity");
            var batteryStatus    = GetWmiValue<ushort>(_managementScopeCimv2, "Win32_Battery", "BatteryStatus");
            var estimatedChargeRemaining =
                GetWmiValue<ushort>(_managementScopeCimv2, "Win32_Battery", "EstimatedChargeRemaining");
            var estimatedRunTime = GetWmiValue<uint>(_managementScopeCimv2, "Win32_Battery", "EstimatedRunTime");

            var  dischargeRate = GetWmiValue<int>(_managementScopeWmi, "BatteryStatus", "DischargeRate");
            var isDischarging = GetWmiValue<bool>(_managementScopeWmi, "BatteryStatus", "Discharging");

            return new PowerLoad(fullChargedCapacity, designedCapacity, batteryStatus, estimatedChargeRemaining,
                estimatedRunTime, dischargeRate, isDischarging);
        }

        private T? GetWmiValue<T>(string scope, string className, string propertyName)
        {
            try
            {
                using var searcher = new ManagementObjectSearcher(scope, $"SELECT {propertyName} FROM {className}");
                foreach (var obj in searcher.Get())
                {
                    return (T)obj[propertyName];
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error retrieving {propertyName} from {className}: {ex.Message}");
            }

            return default;
        }
    }
}