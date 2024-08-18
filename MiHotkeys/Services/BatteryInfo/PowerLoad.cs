namespace MiHotkeys.Services.BatteryInfo
{
    public class PowerLoad
    {
        public uint   FullChargeCapacity       { get; }
        public uint   DesignCapacity           { get; }
        public ushort BatteryStatus            { get; }
        public ushort EstimatedChargeRemaining { get; }
        public uint   EstimatedRunTime         { get; }
        public int    DischargeRate            { get; }
        public bool   IsDischarging            { get; }

        public PowerLoad(uint   fullChargeCapacity,       uint designCapacity,   ushort batteryStatus,
                         ushort estimatedChargeRemaining, uint estimatedRunTime, int dischargeRate, bool isDischarging)
        {
            FullChargeCapacity       = fullChargeCapacity;
            DesignCapacity           = designCapacity;
            BatteryStatus            = batteryStatus;
            EstimatedChargeRemaining = estimatedChargeRemaining;
            EstimatedRunTime         = estimatedRunTime;
            DischargeRate            = dischargeRate;
            IsDischarging            = isDischarging;
        }

        public int CalculatePowerLoadIndex()
        {
            const double minDischargeRate = 8000;
            const double maxDischargeRate = 80000;

            var dischargeRateNormalized =
                Math.Clamp((DischargeRate - minDischargeRate) / (maxDischargeRate - minDischargeRate), 0, 1);

            var loadIndex = (int)(dischargeRateNormalized * 9) + 1;

            return loadIndex;
        }


        public string GetBatteryStatusText()
        {
            return BatteryStatus switch
            {
                1 => "Discharging",
                2 => "Charging",
                3 => "Fully Charged",
                _ => "Unknown"
            };
        }

        public string GetEstimatedRunTimeText()
        {
            return $"{EstimatedRunTime} minutes remaining";
        }
    }
}