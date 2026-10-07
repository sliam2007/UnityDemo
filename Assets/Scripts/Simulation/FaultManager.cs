using UnityEngine;

namespace UnityDemo.Simulation
{
    public class FaultManager
    {
        public bool CoolingFailureActive { get; private set; }

        public void InjectCoolingFailure()
        {
            CoolingFailureActive = true;
        }

        public void RestoreCooling()
        {
            CoolingFailureActive = false;
        }

        public bool ShouldTriggerOverheat(SensorModel sensors)
        {
            return sensors.TemperatureCelsius >= 100f;
        }

        public bool CanReset(SensorModel sensors)
        {
            return !CoolingFailureActive
                && sensors.TemperatureCelsius <= 60f
                && sensors.MotorSpeedRpm <= 0f;
        }
    }
}
