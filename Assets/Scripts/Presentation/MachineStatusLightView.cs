using UnityEngine;
using UnityDemo.Simulation;

namespace UnityDemo.Presentation
{
    public class MachineStatusLightView : MonoBehaviour
    {
        [SerializeField]
        private SimulationController simulationController;

        private Renderer statusRenderer;
        private Material statusMaterial;
        private DeviceState displayedState;
        private bool stateWasDisplayed;

        private void Awake()
        {
            statusRenderer = GetComponent<Renderer>();  
            statusMaterial = statusRenderer.material;
            statusMaterial.EnableKeyword("_EMISSION");
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

            //statusRenderer.material.color =
            //    GetColor(currentState);

            Color color = GetColor(currentState);

            float emissionIntensity =
                currentState == DeviceState.Off ? 0f : 2f;

            statusMaterial.color = color;
            statusMaterial.SetColor(
                "_EmissionColor",
                color * emissionIntensity);
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