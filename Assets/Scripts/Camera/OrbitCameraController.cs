using UnityEngine;
using UnityEngine.InputSystem;

namespace TowerDefense.CameraSystem
{
    [DisallowMultipleComponent]
    public class OrbitCameraController : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform pivot;

        [Header("Rotation")]
        [SerializeField] private float rotationSpeed = 0.2f;
        [SerializeField] private float minPitch = 10f;
        [SerializeField] private float maxPitch = 80f;

        [Header("Zoom")]
        [SerializeField] private float zoomSpeed = 2f;
        [SerializeField] private float minDistance = 3f;
        [SerializeField] private float maxDistance = 20f;

        [Header("Start Values")]
        [SerializeField] private float startYaw = 0f;
        [SerializeField] private float startPitch = 45f;
        [SerializeField] private float startDistance = 10f;

        private float currentYaw;
        private float currentPitch;
        private float currentDistance;

        private void Awake()
        {
            currentYaw = startYaw;
            currentPitch = startPitch;
            currentDistance = startDistance;
        }

        private void LateUpdate()
        {
            if (pivot == null)
            {
                return;
            }

            HandleRotationInput();
            HandleZoomInput();
            ApplyTransform();
        }

        private void HandleRotationInput()
        {
            if (Mouse.current == null)
            {
                return;
            }

            if (!Mouse.current.rightButton.isPressed)
            {
                return;
            }

            Vector2 mouseDelta = Mouse.current.delta.ReadValue();

            currentYaw += mouseDelta.x * rotationSpeed;
            currentPitch -= mouseDelta.y * rotationSpeed;
            currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);
        }

        private void HandleZoomInput()
        {
            if (Mouse.current == null)
            {
                return;
            }

            float scrollDelta = Mouse.current.scroll.ReadValue().y;

            if (Mathf.Approximately(scrollDelta, 0f))
            {
                return;
            }

            currentDistance -= scrollDelta * zoomSpeed * 0.01f;
            currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
        }

        private void ApplyTransform()
        {
            Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
            Vector3 offset = rotation * new Vector3(0f, 0f, -currentDistance);

            transform.position = pivot.position + offset;
            transform.LookAt(pivot.position);
        }
    }
}