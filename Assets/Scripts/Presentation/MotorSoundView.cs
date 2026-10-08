using UnityEngine;
using UnityDemo.Simulation;

namespace UnityDemo.Presentation
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioSource))]
    public sealed class MotorSoundView : MonoBehaviour
    {
        private const float MaximumMotorSpeedRpm = 600f;
        private const float MinimumPitch = 0.6f;
        private const float MaximumPitch = 1f;
        private const float StartupVolume = 0.027f;
        private const float MaximumVolume = 0.9f;
        private const float StartupEnvelopeDurationSeconds = 0.35f;
        private const float CoastFadeStartFraction = 0.5f;
        private const float AudibleCoastDownThresholdRpm = 0f;
        private const double StartSchedulingDelaySeconds = 0.01;

        private SimulationController simulationController;
        private AudioSource motorAudioSource;
        private bool soundIsActive;
        private bool wasRunning;
        private float coastStartRpm;
        private float coastStartVolume;

        private void Awake()
        {
            simulationController =
                FindFirstObjectByType<SimulationController>();

            motorAudioSource = GetComponent<AudioSource>();
            motorAudioSource.loop = true;
            motorAudioSource.playOnAwake = false;
            motorAudioSource.volume = 1f;
            motorAudioSource.pitch = 1f;

            if (motorAudioSource.clip != null)
            {
                motorAudioSource.clip.LoadAudioData();
            }
        }

        private void Update()
        {
            if (simulationController == null
                || simulationController.Sensors == null
                || motorAudioSource.clip == null)
            {
                return;
            }

            bool isRunning =
                simulationController.CurrentState == DeviceState.Running;

            float motorSpeedRpm =
                simulationController.Sensors.MotorSpeedRpm;

            if (wasRunning
                && !isRunning
                && motorSpeedRpm > AudibleCoastDownThresholdRpm)
            {
                coastStartRpm = motorSpeedRpm;
                coastStartVolume = motorAudioSource.volume;
            }

            wasRunning = isRunning;

            bool shouldBeAudible =
                isRunning
                || motorSpeedRpm > AudibleCoastDownThresholdRpm;

            float speedRatio = Mathf.Clamp01(
                motorSpeedRpm / MaximumMotorSpeedRpm);

            motorAudioSource.pitch = Mathf.Lerp(
                MinimumPitch,
                MaximumPitch,
                Mathf.Sqrt(speedRatio));

            if (!shouldBeAudible)
            {
                if (soundIsActive)
                {
                    motorAudioSource.Stop();
                    soundIsActive = false;
                }

                return;
            }

            if (!soundIsActive)
            {
                motorAudioSource.timeSamples = 0;
                motorAudioSource.volume = StartupVolume;
                motorAudioSource.PlayScheduled(
                    AudioSettings.dspTime
                    + StartSchedulingDelaySeconds);
                soundIsActive = true;
            }

            if (isRunning)
            {
                float volumeRisePerSecond =
                    (MaximumVolume - StartupVolume)
                    / StartupEnvelopeDurationSeconds;

                motorAudioSource.volume = Mathf.MoveTowards(
                    motorAudioSource.volume,
                    MaximumVolume,
                    volumeRisePerSecond * Time.deltaTime);
            }
            else if (coastStartRpm > 0f)
            {
                float coastRatio = Mathf.Clamp01(
                    motorSpeedRpm
                    / (coastStartRpm * CoastFadeStartFraction));

                motorAudioSource.volume =
                    coastStartVolume * coastRatio;
            }
        }
    }
}
