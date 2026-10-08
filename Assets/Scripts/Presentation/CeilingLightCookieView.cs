using UnityEngine;

namespace UnityDemo.Presentation
{
    internal static class CeilingLightCookieView
    {
        private const int TextureSize = 256;
        private const string FixturesPath = "WorldBounds/CeilingLights";

        [RuntimeInitializeOnLoadMethod(
            RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void ApplyOvalCookies()
        {
            GameObject fixtures = GameObject.Find(FixturesPath);

            if (fixtures == null)
            {
                Debug.LogError("Ceiling light fixtures were not found.");
                return;
            }

            Texture2D cookie = CreateOvalCookie();
            int assignedCount = 0;

            foreach (Transform fixture in fixtures.transform)
            {
                Light spotLight = fixture.GetComponentInChildren<Light>(true);

                if (spotLight == null || spotLight.type != LightType.Spot)
                {
                    continue;
                }

                spotLight.cookie = cookie;
                assignedCount++;
            }

            if (assignedCount != 6)
            {
                Debug.LogError(
                    $"Expected 6 ceiling spot lights, found {assignedCount}.");
            }
        }

        private static Texture2D CreateOvalCookie()
        {
            Texture2D cookie = new Texture2D(
                TextureSize,
                TextureSize,
                TextureFormat.RGBA32,
                false,
                true);
            cookie.name = "CeilingLightOvalCookieRuntime";
            cookie.wrapMode = TextureWrapMode.Clamp;
            cookie.filterMode = FilterMode.Bilinear;

            Color[] pixels = new Color[TextureSize * TextureSize];

            for (int y = 0; y < TextureSize; y++)
            {
                float vertical = ((y + 0.5f) / TextureSize) * 2f - 1f;

                for (int x = 0; x < TextureSize; x++)
                {
                    float horizontal = ((x + 0.5f) / TextureSize) * 2f - 1f;
                    float distance = Mathf.Sqrt(
                        horizontal * horizontal / (0.97f * 0.97f) +
                        vertical * vertical / (0.35f * 0.35f));
                    float fade = Mathf.InverseLerp(0.30f, 1.00f, distance);
                    float brightness = 1f - fade * fade * (3f - 2f * fade);
                    pixels[y * TextureSize + x] = new Color(
                        brightness,
                        brightness,
                        brightness,
                        brightness);
                }
            }

            cookie.SetPixels(pixels);
            cookie.Apply(false, true);
            return cookie;
        }
    }
}