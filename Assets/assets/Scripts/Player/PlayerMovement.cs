using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Walking")]
    [SerializeField] private float walkSpeed = 3.5f;

    [Header("Sprint")]
    [SerializeField] private bool sprintEnabled = true;
    [SerializeField] private float sprintSpeed = 5.5f;
    [SerializeField] private float sprintThreshold = 0.1f;

    [Header("Gravity")]
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float groundedVelocity = -2f;

    private CharacterController controller;
    private Vector3 verticalVelocity;

    public Vector3 MoveDirection { get; private set; }
    public bool IsMoving { get; private set; }
    public bool IsSprinting { get; private set; }
    public bool IsGrounded => controller.isGrounded;

    public float CurrentSpeed { get; private set; }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        ReadMovementInput();
        ApplyMovement();
        ApplyGravity();
    }

    private void ReadMovementInput()
    {
        Vector2 input = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed)
                input.y += 1f;

            if (Keyboard.current.sKey.isPressed)
                input.y -= 1f;

            if (Keyboard.current.dKey.isPressed)
                input.x += 1f;

            if (Keyboard.current.aKey.isPressed)
                input.x -= 1f;
        }

        input = Vector2.ClampMagnitude(input, 1f);

        MoveDirection =
            transform.right * input.x +
            transform.forward * input.y;

        IsMoving = input.sqrMagnitude > 0.01f;

        IsSprinting =
            sprintEnabled &&
            IsMoving &&
            Keyboard.current != null &&
            Keyboard.current.leftShiftKey.isPressed;

        CurrentSpeed = IsSprinting
            ? sprintSpeed
            : walkSpeed;
    }

    private void ApplyMovement()
    {
        controller.Move(
            MoveDirection *
            CurrentSpeed *
            Time.deltaTime
        );
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded && verticalVelocity.y < 0f)
        {
            verticalVelocity.y = groundedVelocity;
        }

        verticalVelocity.y += gravity * Time.deltaTime;

        controller.Move(
            verticalVelocity *
            Time.deltaTime
        );
    }
}