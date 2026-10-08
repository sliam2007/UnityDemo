using UnityEngine;

namespace UnityDemo.Presentation
{
    public sealed class FaultyCeilingLightView : MonoBehaviour
    {
        private const string TargetLightPath =
            "WorldBounds/CeilingLights/CeilingLight03";
        private const string PanelName = "LuminousPanel";
        private const float EmissionIntensity = 8f;

        private static readonly Color PanelColor =
            new Color(0.9f, 0.95f, 1f);

        private Renderer panelRenderer;
        private Material panelMaterial;
        private Light spotLight;
        private bool sequenceActive;
        private bool isDark;
        private int darkPulsesRemaining;
        private float nextSwitchTime;

        [RuntimeInitializeOnLoadMethod(
            RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            GameObject targetLight = GameObject.Find(TargetLightPath);

            if (targetLight != null &&
                targetLight.GetComponent<FaultyCeilingLightView>() == null)
            {
                targetLight.AddComponent<FaultyCeilingLightView>();
            }
        }

        private void Awake()
        {
            Transform panel = transform.Find(PanelName);
            panelRenderer = panel == null
                ? null
                : panel.GetComponent<Renderer>();
            spotLight = GetComponentInChildren<Light>(true);

            if (panelRenderer != null)
            {
                panelMaterial = panelRenderer.material;
                panelMaterial.EnableKeyword("_EMISSION");
            }

            SetPowered(true);
            ScheduleNextFailure();
        }

        private void Update()
        {
            if (Time.unscaledTime < nextSwitchTime)
            {
                return;
            }

            if (!sequenceActive)
            {
                sequenceActive = true;
                darkPulsesRemaining = Random.Range(2, 4);
                SetPowered(false);
                nextSwitchTime =
                    Time.unscaledTime + Random.Range(0.06f, 0.16f);
                return;
            }

            if (isDark)
            {
                SetPowered(true);
                darkPulsesRemaining--;

                if (darkPulsesRemaining <= 0)
                {
                    sequenceActive = false;
                    ScheduleNextFailure();
                }
                else
                {
                    nextSwitchTime =
                        Time.unscaledTime + Random.Range(0.04f, 0.14f);
                }

                return;
            }

            SetPowered(false);
            nextSwitchTime =
                Time.unscaledTime + Random.Range(0.05f, 0.14f);
        }

        private void OnDisable()
        {
            SetPowered(true);
        }

        private void SetPowered(bool powered)
        {
            isDark = !powered;

            if (spotLight != null)
            {
                spotLight.enabled = powered;
            }

            if (panelMaterial != null)
            {
                panelMaterial.SetColor(
                    "_EmissionColor",
                    powered
                        ? PanelColor * EmissionIntensity
                        : Color.black);
            }
        }

        private void ScheduleNextFailure()
        {
            nextSwitchTime =
                Time.unscaledTime + Random.Range(2f, 6f);
        }
    }
}