using System;

namespace UnityDemo.Simulation
{
    public sealed class SensorModel
    {
        private const float AmbientTemperature = 22f;
        private const float RunningTemperature = 75f;
        private const float RunningPressure = 6.5f;
        private const float RunningMotorSpeed = 600f;

        public float TemperatureCelsius { get; private set; }
            = AmbientTemperature;

        public float PressureBar { get; private set; }

        public float MotorSpeedRpm { get; private set; }

        public void Tick(float deltaTime, bool isRunning, bool coolingFailureActive)
        {
            float operatingTemperature = coolingFailureActive ? 140F : RunningTemperature;

            float targetTemperature =
                isRunning ? operatingTemperature : AmbientTemperature;

            float temperatureRate = isRunning ? 8f : 4f;

            TemperatureCelsius = MoveTowards(
                TemperatureCelsius,
                targetTemperature,
                temperatureRate * deltaTime);

            PressureBar = MoveTowards(
                PressureBar,
                isRunning ? RunningPressure : 0f,
                1.5f * deltaTime);

            float motorSpeedChangeRate = isRunning ? 600f : 300f;

            MotorSpeedRpm = MoveTowards(
                MotorSpeedRpm,
                isRunning ? RunningMotorSpeed : 0f,
                motorSpeedChangeRate * deltaTime);
        }

        public void StopMotorImmediately()
        {
            MotorSpeedRpm = 0f;
        }

        private static float MoveTowards(
            float current,
            float target,
            float maximumChange)
        {
            float difference = target - current;

            if (Math.Abs(difference) <= maximumChange)
            {
                return target;
            }

            return current + Math.Sign(difference) * maximumChange;
        }
    }
}
