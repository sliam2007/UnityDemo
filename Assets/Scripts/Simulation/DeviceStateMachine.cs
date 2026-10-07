using System;

namespace UnityDemo.Simulation
{
    public sealed class DeviceStateMachine
    {
        public DeviceState CurrentState { get; private set; } = DeviceState.Off;

        public event Action<DeviceState> StateChanged;

        public bool PowerOn()
        {
            return TryTransition(DeviceState.Off, DeviceState.Ready);
        }

        public bool PowerOff()
        {
            return TryTransition(DeviceState.Ready, DeviceState.Off);
        }

        public bool Start()
        {
            return TryTransition(DeviceState.Ready, DeviceState.Running);
        }

        public bool Stop()
        {
            return TryTransition(DeviceState.Running, DeviceState.Ready);
        }

        public void TriggerFault()
        {
            if (CurrentState == DeviceState.EmergencyStop)
            {
                return;
            }

            SetState(DeviceState.Fault);
        }

        public void TriggerEmergencyStop()
        {
            SetState(DeviceState.EmergencyStop);
        }

        public bool Reset()
        {
            if (CurrentState == DeviceState.Running ||
                CurrentState == DeviceState.Off)
            {
                return false;
            }

            SetState(DeviceState.Off);
            return true;
        }

        private bool TryTransition(DeviceState requiredState, DeviceState newState)
        {
            if (CurrentState != requiredState)
            {
                return false;
            }

            SetState(newState);
            return true;
        }

        private void SetState(DeviceState newState)
        {
            if (CurrentState == newState)
            {
                return;
            }

            CurrentState = newState;
            StateChanged?.Invoke(CurrentState);
        }
    }
}