using UnityEngine;
using UnityDemo.Simulation;

namespace UnityDemo.Presentation
{
    public class MachineStatusLightView : MonoBehaviour
    {
        [SerializeField]
        private SimulationController simulationController;

        private Renderer statusRenderer;
        private DeviceState displayedState;
        private bool stateWasDisplayed;

        private void Awake()
        {
            statusRenderer = GetComponent<Renderer>();  
        }

        private void Update()
        {
            if (simulationController==null)
            {
                return;
            }

            DeviceState currentState =
                simulationController.CurrentState;

            if (stateWasDisplayed &&
                displayedState == currentState)
            {
                return;
            }

            displayedState = currentState;
            stateWasDisplayed = true;

            statusRenderer.material.color =
                GetColor(currentState);
        }

        private static Color GetColor(DeviceState state)
        {
            switch (state)
            {
                case DeviceState.Off:
                    return Color.gray;

                case DeviceState.Ready:
                    return Color.yellow;

                case DeviceState.Running:
                    return Color.green;

                case DeviceState.Fault:
                    return Color.red;

                case DeviceState.EmergencyStop:
                    return Color.magenta;

                default:
                    return Color.white;

            }
        }
    }
}