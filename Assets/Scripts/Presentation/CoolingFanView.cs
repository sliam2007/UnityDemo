using UnityEngine;
using UnityDemo.Simulation;

namespace UnityDemo.Presentation
{
    [DisallowMultipleComponent]
    public sealed class CoolingFanView : MonoBehaviour
    {
        private const float RunningRpm = 100f;
        private const float AccelerationRpmPerSecond = 560f;
        private const float DecelerationRpmPerSecond = 25f;

        private SimulationController simulationController;
        private Transform fanRotor;
        private float currentRpm;

        public float CurrentRpm => currentRpm;

        private void Awake()
        {
            simulationController =
                FindFirstObjectByType<SimulationController>();

            fanRotor = transform.Find("FanRotor");
        }

        private void Update()
        {
            if (simulationController == null || fanRotor == null)
            {
                return;
            }

            bool shouldRun =
                simulationController.CurrentState == DeviceState.Running
                && !simulationController.CoolingFailureActive;

            float targetRpm = shouldRun ? RunningRpm : 0f;
            float changeRate = targetRpm > currentRpm
                ? AccelerationRpmPerSecond
                : DecelerationRpmPerSecond;

            currentRpm = Mathf.MoveTowards(
                currentRpm,
                targetRpm,
                changeRate * Time.deltaTime);

            fanRotor.Rotate(
                Vector3.up,
                currentRpm * 6f * Time.deltaTime,
                Space.Self);
        }
    }
}
