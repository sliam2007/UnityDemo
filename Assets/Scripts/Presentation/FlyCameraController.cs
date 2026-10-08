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

        [SerializeField, Min(0.05f)]
        private float collisionRadius = 0.3f;

        [SerializeField, Min(0f)]
        private float collisionSkin = 0.05f;

        [SerializeField, Min(0.01f)]
        private float nearClipDistance = 0.05f;

        [SerializeField]
        private LayerMask collisionMask = ~0;

        private float yaw;
        private float pitch;

        private void Awake()
        {
            Camera controlledCamera = GetComponent<Camera>();

            if (controlledCamera != null)
            {
                controlledCamera.nearClipPlane = nearClipDistance;
            }

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

            Vector3 displacement =
                direction.normalized * currentSpeed * Time.deltaTime;

            MoveWithCollisions(displacement);
        }

        private void MoveWithCollisions(Vector3 displacement)
        {
            Vector3 position = transform.position;
            Vector3 remainingMovement = displacement;

            const int maximumCollisionPasses = 3;

            for (int pass = 0;
                 pass < maximumCollisionPasses;
                 pass++)
            {
                float distance = remainingMovement.magnitude;

                if (distance <= Mathf.Epsilon)
                {
                    break;
                }

                Vector3 direction = remainingMovement / distance;

                if (!Physics.SphereCast(
                        position,
                        collisionRadius,
                        direction,
                        out RaycastHit hit,
                        distance + collisionSkin,
                        collisionMask,
                        QueryTriggerInteraction.Ignore))
                {
                    position += remainingMovement;
                    break;
                }

                float safeDistance = Mathf.Max(
                    hit.distance - collisionSkin,
                    0f);

                Vector3 completedMovement =
                    direction * safeDistance;

                position += completedMovement;
                remainingMovement -= completedMovement;
                remainingMovement = Vector3.ProjectOnPlane(
                    remainingMovement,
                    hit.normal);
            }

            transform.position = position;
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
