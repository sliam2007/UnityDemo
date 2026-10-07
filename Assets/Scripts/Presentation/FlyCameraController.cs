using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityDemo.Presentation
{
    public sealed class FlyCameraController : MonoBehaviour
    {
        [SerializeField, Min(0.1f)]
        private float moveSpeed = 4f;

        [SerializeField, Min(1f)]
        private float sprintMultiplier = 2.5f;

        [SerializeField, Min(0.01f)]
        private float lookSensitivity = 0.12f;

        private float yaw;
        private float pitch;

        private void Awake()
        {
            Vector3 angles = transform.eulerAngles;

            yaw = angles.y;
            pitch = angles.x > 180f
                ? angles.x - 360f
                : angles.x;
        }

        private void Update()
        {
            Mouse mouse = Mouse.current;
            Keyboard keyboard = Keyboard.current;

            if (mouse == null || keyboard == null)
            {
                return;
            }

            if (mouse.rightButton.wasPressedThisFrame)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            if (mouse.rightButton.wasReleasedThisFrame)
            {
                ReleaseCursor();
            }

            if (!mouse.rightButton.isPressed)
            {
                return;
            }

            UpdateRotation(mouse);
            UpdatePosition(keyboard);
        }

        private void UpdateRotation(Mouse mouse)
        {
            Vector2 mouseDelta = mouse.delta.ReadValue();

            yaw += mouseDelta.x * lookSensitivity;
            pitch -= mouseDelta.y * lookSensitivity;
            pitch = Mathf.Clamp(pitch, -85f, 85f);

            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }

        private void UpdatePosition(Keyboard keyboard)
        {
            Vector3 direction = Vector3.zero;

            if (keyboard.wKey.isPressed)
                direction += transform.forward;

            if (keyboard.sKey.isPressed)
                direction -= transform.forward;

            if (keyboard.dKey.isPressed)
                direction += transform.right;

            if (keyboard.aKey.isPressed)
                direction -= transform.right;

            if (keyboard.eKey.isPressed)
                direction += Vector3.up;

            if (keyboard.qKey.isPressed)
                direction -= Vector3.up;

            bool sprinting =
                keyboard.leftShiftKey.isPressed ||
                keyboard.rightShiftKey.isPressed;

            float currentSpeed = sprinting
                ? moveSpeed * sprintMultiplier
                : moveSpeed;

            transform.position +=
                direction.normalized * currentSpeed * Time.deltaTime;
        }

        private void OnDisable()
        {
            ReleaseCursor();
        }

        private static void ReleaseCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}