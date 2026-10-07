using TMPro;
using UnityEngine;
using UnityDemo.Simulation;

namespace UnityDemo.Presentation
{
    public sealed class TelemetryView : MonoBehaviour
    {
        [SerializeField]
        private SimulationController simulationController;

        [SerializeField]
        private TMP_Text stateText;

        [SerializeField]
        private TMP_Text temperatureText;

        [SerializeField]
        private TMP_Text pressureText;

        [SerializeField]
        private TMP_Text motorSpeedText;

        [SerializeField]
        private TMP_Text messageText;

        [SerializeField, Min(0.05f)]
        private float refreshInterval = 0.1f;

        private float timeUntilRefresh;

        private void Update()
        {
            timeUntilRefresh -= Time.deltaTime;

            if (timeUntilRefresh > 0f)
            {
                return;
            }

            timeUntilRefresh = refreshInterval;
            Refresh();
        }

        private void Refresh()
        {
            if (simulationController == null ||
                simulationController.Sensors == null)
            {
                return;
            }

            SensorModel sensors = simulationController.Sensors;

            stateText.text =
                $"STATE: {simulationController.CurrentState}";

            temperatureText.text =
                $"TEMPERATURE: {sensors.TemperatureCelsius:F1} \u00B0C";

            pressureText.text =
                $"PRESSURE: {sensors.PressureBar:F1} bar";

            motorSpeedText.text =
                $"MOTOR SPEED: {sensors.MotorSpeedRpm:F0} RPM";

            messageText.text = simulationController.OperatorMessage;
        }
    }
}