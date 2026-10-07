using UnityEngine;
using UnityEngine.UI;
using UnityDemo.Presentation;
using UnityDemo.Simulation;

namespace UnityDemo.Presentation
{
    public class OperatorPanelView : MonoBehaviour
    {
        [SerializeField]
        private SimulationController simulationController;

        [SerializeField]
        private Button powerOnButton;

        [SerializeField]
        private Button powerOffButton;

        [SerializeField]
        private Button startButton;

        [SerializeField]
        private Button stopButton;

        [SerializeField]
        private Button resetButton;

        [SerializeField]
        private Button emergencyStopButton;

        [SerializeField]
        private Button overheatButton;

        [SerializeField]
        private Button restoreCoolingButton;

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
            if (simulationController == null)
                return;

            DeviceState state = simulationController.CurrentState;
            bool coolingFailure = 
                simulationController.CoolingFailureActive;

            powerOnButton.interactable = 
                state == DeviceState.Off;

            powerOffButton.interactable =
                simulationController.CanPowerOff;

            startButton.interactable =
                state == DeviceState.Ready
                && !coolingFailure;

            stopButton.interactable =
                state == DeviceState.Running;

            resetButton.interactable =
                (state == DeviceState.Fault
                || state == DeviceState.EmergencyStop)
                && simulationController.CanReset;

            emergencyStopButton.interactable = 
                state == DeviceState.Running;

            restoreCoolingButton.interactable =
                coolingFailure
                && state != DeviceState.Running;

            overheatButton.interactable =
                state == DeviceState.Running
                && !coolingFailure;



        }
    }
}
