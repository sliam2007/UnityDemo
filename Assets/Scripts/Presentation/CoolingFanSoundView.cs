using UnityEngine;

namespace UnityDemo.Presentation
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioSource))]
    public sealed class CoolingFanSoundView : MonoBehaviour
    {
        private const float MaximumFanSpeedRpm = 100f;
        private const float MaximumVolume = 0.8f;
        private const float MinimumPitch = 0.85f;
        private const float MaximumPitch = 1f;
        private const double StartSchedulingDelaySeconds = 0.01;

        private CoolingFanView coolingFan;
        private AudioSource fanAudioSource;
        private bool soundIsActive;

        private void Awake()
        {
            coolingFan = GetComponent<CoolingFanView>();
            fanAudioSource = GetComponent<AudioSource>();

            fanAudioSource.loop = true;
            fanAudioSource.playOnAwake = false;
            fanAudioSource.spatialBlend = 0.7f;
            fanAudioSource.minDistance = 4f;

            if (fanAudioSource.clip != null)
            {
                fanAudioSource.clip.LoadAudioData();
            }
        }

        private void Update()
        {
            if (coolingFan == null || fanAudioSource.clip == null)
            {
                return;
            }

            float speedRatio = Mathf.Clamp01(
                coolingFan.CurrentRpm / MaximumFanSpeedRpm);

            if (speedRatio <= 0f)
            {
                if (soundIsActive)
                {
                    fanAudioSource.Stop();
                    soundIsActive = false;
                }

                return;
            }

            fanAudioSource.volume = MaximumVolume * speedRatio;
            fanAudioSource.pitch = Mathf.Lerp(
                MinimumPitch,
                MaximumPitch,
                speedRatio);

            if (!soundIsActive)
            {
                fanAudioSource.timeSamples = 0;
                fanAudioSource.PlayScheduled(
                    AudioSettings.dspTime
                    + StartSchedulingDelaySeconds);
                soundIsActive = true;
            }
        }
    }
}
