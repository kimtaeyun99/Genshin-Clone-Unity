using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Camera")]
    [SerializeField] private float distance = 5f;
    [SerializeField] private float height = 2f;
    [SerializeField] private float sensitivity = 0.15f;

    [Header("Vertical Rotation")]
    [SerializeField] private float minPitch = -30f;
    [SerializeField] private float maxPitch = 60f;

    [Header("Camera Collision")]
    [SerializeField] private LayerMask collisionLayer;
    [SerializeField] private float collisionRadius = 0.2f;
    [SerializeField] private float collisionOffset = 0.1f;

    private float yaw;
    private float pitch;

    private void Start()
    {
        Vector3 angles = transform.eulerAngles;

        yaw = angles.y;
        pitch = angles.x;
    }

    private void LateUpdate()
    {
        RotateCamera();
    }

    private void RotateCamera()
    {
        if (target == null)
        {
            return;
        }
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            return;
        }

        Vector2 lookInput =
            InputManager.Instance.LookInput;

        yaw += lookInput.x * sensitivity;
        pitch -= lookInput.y * sensitivity;

        pitch = Mathf.Clamp(
            pitch,
            minPitch,
            maxPitch
        );

        Quaternion rotation =
            Quaternion.Euler(
                pitch,
                yaw,
                0f
            );

        Vector3 targetPosition =
            target.position +
            Vector3.up * height;

        Vector3 direction =
            -(rotation * Vector3.forward);

        float cameraDistance = distance;

        if (Physics.SphereCast(
            targetPosition,
            collisionRadius,
            direction,
            out RaycastHit hit,
            distance,
            collisionLayer,
            QueryTriggerInteraction.Ignore))
        {
            cameraDistance =
                Mathf.Max(
                    hit.distance - collisionOffset,
                    0f
                );
        }

        transform.position =
            targetPosition +
            direction * cameraDistance;

        transform.rotation = rotation;
    }
}