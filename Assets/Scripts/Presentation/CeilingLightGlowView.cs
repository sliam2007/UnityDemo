using UnityEngine;

namespace UnityDemo.Presentation
{
    internal static class CeilingLightGlowView
    {
        private const string PanelName = "LuminousPanel";
        private const float EmissionIntensity = 8f;

        [RuntimeInitializeOnLoadMethod(
            RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void ApplyGlow()
        {
            Renderer[] renderers =
                Object.FindObjectsByType<Renderer>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None);

            foreach (Renderer panelRenderer in renderers)
            {
                if (panelRenderer.gameObject.name != PanelName)
                {
                    continue;
                }

                Material panelMaterial = panelRenderer.material;
                Color panelColor = new Color(0.9f, 0.95f, 1f);

                panelMaterial.EnableKeyword("_EMISSION");
                panelMaterial.SetColor("_BaseColor", Color.white);
                panelMaterial.SetColor(
                    "_EmissionColor",
                    panelColor * EmissionIntensity);
            }
        }
    }
}
