using LibreHardwareMonitor.Hardware;

namespace SimpleGameOverlay
{
    public class HardwareMonitorService
    {
        private readonly Computer _computer;

        public HardwareMonitorService()
        {
            _computer = new Computer
            {
                IsCpuEnabled = true,
                IsGpuEnabled = true,
            };
            _computer.Open();
        }

        public HardwareData GetData()
        {
            var data = new HardwareData();

            foreach (var hardware in _computer.Hardware)
            {
                hardware.Update();

                if (hardware.HardwareType == HardwareType.Cpu)
                {
                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Load && sensor.Name.Contains("Total"))
                            data.CpuLoad = sensor.Value ?? 0;

                        if (sensor.SensorType == SensorType.Temperature &&
                            (sensor.Name.Contains("Package") || sensor.Name.Contains("Tdie")))
                            data.CpuTemp = sensor.Value ?? 0;
                    }
                }

                if (hardware.HardwareType == HardwareType.GpuNvidia ||
                    hardware.HardwareType == HardwareType.GpuAmd ||
                    hardware.HardwareType == HardwareType.GpuIntel)
                {
                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Load && sensor.Name.Contains("Core"))
                            data.GpuLoad = sensor.Value ?? 0;

                        if (sensor.SensorType == SensorType.Temperature &&
                            (sensor.Name.Contains("Core") || sensor.Name.Contains("Hot Spot") == false))
                            data.GpuTemp = sensor.Value ?? 0;
                    }
                }
            }

            return data;
        }

        public void Close()
        {
            _computer.Close();
        }
    }

    public class HardwareData
    {
        public float CpuLoad { get; set; }
        public float CpuTemp { get; set; }
        public float GpuLoad { get; set; }
        public float GpuTemp { get; set; }
    }
}