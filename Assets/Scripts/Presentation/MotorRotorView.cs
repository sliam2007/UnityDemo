using UnityEngine;

namespace UnityDemo.Presentation
{
    public sealed class MotorRotorView : MonoBehaviour
    {
        [SerializeField, Min(0f)]
        private float rotationsPerMinute;

        public float RotationsPerMinute
        { 
            get => rotationsPerMinute;
            set => rotationsPerMinute = Mathf.Max(value, 0f);
        }

        private void Update()
        {
            if (rotationsPerMinute <= 0)
            {
                return;
            }

            float degreesPerSecond = rotationsPerMinute * 6f;

            transform.Rotate(
                Vector3.up,
                degreesPerSecond * Time.deltaTime,
                Space.Self);
        }
    }
}
