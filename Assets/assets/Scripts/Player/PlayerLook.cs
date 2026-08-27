using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    [Header("Look")]
    [SerializeField] private float sensitivity = 0.08f;
    [SerializeField] private float minVerticalAngle = -80f;
    [SerializeField] private float maxVerticalAngle = 80f;

    [Header("Camera Bob")]
    [SerializeField] private bool enableHeadBob = true;

    [SerializeField] private float bobFrequency = 7f;

    [SerializeField] private float bobHorizontalAmount = 0.025f;

    [SerializeField] private float bobVerticalAmount = 0.035f;

    [SerializeField] private float bobSmoothness = 10f;

    [Header("Camera Rotation")]
    [SerializeField] private bool enableCameraRotation = true;

    [SerializeField] private float rotationFrequency = 7f;

    [SerializeField] private float walkingRollAmount = 1.0f;

    [SerializeField] private float walkingPitchAmount = 0.5f;

    [SerializeField] private float sprintRotationMultiplier = 1.25f;

    [SerializeField] private float rotationSmoothness = 8f;

    private PlayerMovement movement;

    private float verticalRotation;
    private float bobTimer;

    private Vector3 cameraInitialPosition;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();

        if (cameraTransform != null)
        {
            cameraInitialPosition =
                cameraTransform.localPosition;
        }
    }

    private void Update()
    {
        HandleLook();
        HandleCameraBob();
        HandleCameraRotation();
    }

    private void HandleLook()
    {
        if (Mouse.current == null)
            return;

        Vector2 mouseDelta =
            Mouse.current.delta.ReadValue();

        float mouseX =
            mouseDelta.x * sensitivity;

        float mouseY =
            mouseDelta.y * sensitivity;

        verticalRotation -= mouseY;

        verticalRotation = Mathf.Clamp(
            verticalRotation,
            minVerticalAngle,
            maxVerticalAngle
        );

        transform.Rotate(
            Vector3.up * mouseX
        );
    }

    private void HandleCameraBob()
    {
        if (!enableHeadBob ||
            movement == null ||
            cameraTransform == null)
        {
            return;
        }

        if (!movement.IsMoving ||
            !movement.IsGrounded)
        {
            ResetBob();
            return;
        }

        float currentFrequency =
            movement.IsSprinting
                ? bobFrequency * 1.25f
                : bobFrequency;

        float currentHorizontalAmount =
            movement.IsSprinting
                ? bobHorizontalAmount * 1.25f
                : bobHorizontalAmount;

        float currentVerticalAmount =
            movement.IsSprinting
                ? bobVerticalAmount * 1.25f
                : bobVerticalAmount;

        bobTimer +=
            Time.deltaTime *
            currentFrequency;

        float horizontalBob =
            Mathf.Cos(bobTimer * 0.5f) *
            currentHorizontalAmount;

        float verticalBob =
            Mathf.Sin(bobTimer) *
            currentVerticalAmount;

        Vector3 targetPosition =
            cameraInitialPosition +
            new Vector3(
                horizontalBob,
                verticalBob,
                0f
            );

        cameraTransform.localPosition =
            Vector3.Lerp(
                cameraTransform.localPosition,
                targetPosition,
                bobSmoothness *
                Time.deltaTime
            );
    }

    private void HandleCameraRotation()
    {
        if (!enableCameraRotation ||
            movement == null ||
            cameraTransform == null)
        {
            return;
        }

        if (!movement.IsMoving ||
            !movement.IsGrounded)
        {
            ResetCameraRotation();
            return;
        }

        float multiplier =
            movement.IsSprinting
                ? sprintRotationMultiplier
                : 1f;

        float currentTime =
            Time.time *
            rotationFrequency;

        float roll =
            Mathf.Sin(currentTime) *
            walkingRollAmount *
            multiplier;

        float pitch =
            Mathf.Cos(currentTime * 2f) *
            walkingPitchAmount *
            multiplier;

        Quaternion targetRotation =
            Quaternion.Euler(
                verticalRotation + pitch,
                0f,
                roll
            );

        cameraTransform.localRotation =
            Quaternion.Slerp(
                cameraTransform.localRotation,
                targetRotation,
                rotationSmoothness *
                Time.deltaTime
            );
    }

    private void ResetBob()
    {
        bobTimer = 0f;

        if (cameraTransform == null)
            return;

        cameraTransform.localPosition =
            Vector3.Lerp(
                cameraTransform.localPosition,
                cameraInitialPosition,
                bobSmoothness *
                Time.deltaTime
            );
    }

    private void ResetCameraRotation()
    {
        Quaternion targetRotation =
            Quaternion.Euler(
                verticalRotation,
                0f,
                0f
            );

        cameraTransform.localRotation =
            Quaternion.Slerp(
                cameraTransform.localRotation,
                targetRotation,
                rotationSmoothness *
                Time.deltaTime
            );
    }
}