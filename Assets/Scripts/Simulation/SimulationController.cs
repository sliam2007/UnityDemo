using UnityEngine;
using UnityDemo.Presentation;

namespace UnityDemo.Simulation
{
    public sealed class SimulationController : MonoBehaviour
    {
        [SerializeField]
        private MotorRotorView motorRotor;

        private DeviceStateMachine stateMachine;

        private FaultManager faultManager;

        public DeviceState CurrentState => stateMachine.CurrentState;

        public SensorModel Sensors { get; private set; }

        public bool CoolingFailureActive => faultManager.CoolingFailureActive;

        public bool CanReset =>
            CurrentState != DeviceState.Off
            && CurrentState != DeviceState.Running
            && faultManager.CanReset(Sensors);

        public bool CanPowerOff =>
            CurrentState == DeviceState.Ready
            && Sensors.MotorSpeedRpm <= 0f;

        public string OperatorMessage { get; private set; }

        private void Awake()
        {
            stateMachine = new DeviceStateMachine();
            Sensors = new SensorModel();
            faultManager = new FaultManager();

            stateMachine.StateChanged += HandleStateChanged;

            HandleStateChanged(stateMachine.CurrentState);
        }
        private void Update()
        {
            bool isRunning = CurrentState == DeviceState.Running;

            Sensors.Tick(Time.deltaTime, isRunning, faultManager.CoolingFailureActive);

            if (isRunning && faultManager.ShouldTriggerOverheat(Sensors))
            {
                stateMachine.TriggerFault();
            }

            if (motorRotor != null)
            {
                motorRotor.RotationsPerMinute = Sensors.MotorSpeedRpm;
            }
        }

        private void OnDestroy()
        {
            stateMachine.StateChanged -= HandleStateChanged;
        }

        public void PowerOn()
        {
            stateMachine.PowerOn();
        }

        public void PowerOff()
        {
            if (CurrentState != DeviceState.Ready)
            {
                OperatorMessage =
                    "Power off is only available in Ready state.";
                return;
            }

            if (Sensors.MotorSpeedRpm > 0f)
            {
                OperatorMessage =
                    "Power off blocked: wait for the motor to stop.";
                return;
            }

            stateMachine.PowerOff();
        }

        public void StartMachine()
        {
            if (Sensors.TemperatureCelsius >= 100f)
            {
                stateMachine.TriggerFault();
                return;
            }

            stateMachine.Start();
        }

        public void StopMachine()
        {
            if (!stateMachine.Stop())
            {
                return;
            }

            if (faultManager.CoolingFailureActive)
            {
                faultManager.RestoreCooling();
            }
        }

        public void TriggerFault()
        {
            stateMachine.TriggerFault();
        }

        public void TriggerEmergencyStop()
        {
            stateMachine.TriggerEmergencyStop();
            Sensors.StopMotorImmediately();

            if (motorRotor != null)
            {
                motorRotor.RotationsPerMinute = 0f;
            }
        }

        public void ResetMachine()
        {
            if (faultManager.CoolingFailureActive)
            {
                OperatorMessage = "Reset blocked: press RESTORE COOLING.";
                return;
            }

            if (Sensors.TemperatureCelsius >= 60f)
            {
                OperatorMessage =
                    "Reset blocked: wait for temperature below 60 \u00B0C.";
                return;
            }

            if (Sensors.MotorSpeedRpm > 0f)
            {
                OperatorMessage = "Reset blocked: wait for the motor to stop.";
                return;
            }

            if (!stateMachine.Reset())
            {
                OperatorMessage = "Reset unavailable in the current state.";
            }
        }

        public void InjectCoolingFailure()
        {
            faultManager.InjectCoolingFailure();
            OperatorMessage = "Cooling failure active.";
        }

        public void RestoreCooling()
        {
            if (CurrentState == DeviceState.Running)
            {
                OperatorMessage = "Stop the machine before restoring cooling.";
                return;
            }

            faultManager.RestoreCooling();
            OperatorMessage = "Cooling restored. Reset requires temperature below 60 \u00B0C "
                + "and a stopped motor.";
        }
        private void HandleStateChanged(DeviceState newState)
        {
            switch (newState)
            {
                case DeviceState.Off:
                    OperatorMessage = "Machine off. Press POWER ON.";
                    break;

                case DeviceState.Ready:
                    OperatorMessage = "Machine ready. Press START.";
                    break;

                case DeviceState.Running:
                    OperatorMessage = "Machine running.";
                    break;

                case DeviceState.Fault:
                    OperatorMessage = "Overheat detected. Restore cooling and wait before reset.";
                    break;

                case DeviceState.EmergencyStop:
                    OperatorMessage = "Emergency stop activated. Check conditions before reset.";
                    break;

            }


            Debug.Log($"Machine state: {newState}");
        }
    }
}
