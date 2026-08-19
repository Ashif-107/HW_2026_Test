using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class DoofusController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float acceleration = 20f;
    [SerializeField] private float deceleration = 25f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 12f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float groundCheckDistance = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody rb;
    private Vector2 inputDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationZ;
    }

    private void Update()
    {
        ReadMovementInput();
        HandleJumpInput();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
    }

    private void ReadMovementInput()
    {
        if (Keyboard.current == null)
        {
            inputDirection = Vector2.zero;
            return;
        }

        float horizontal = 0f;
        float vertical = 0f;

        if (Keyboard.current.aKey.isPressed ||
            Keyboard.current.leftArrowKey.isPressed)
        {
            horizontal -= 1f;
        }

        if (Keyboard.current.dKey.isPressed ||
            Keyboard.current.rightArrowKey.isPressed)
        {
            horizontal += 1f;
        }

        if (Keyboard.current.sKey.isPressed ||
            Keyboard.current.downArrowKey.isPressed)
        {
            vertical -= 1f;
        }

        if (Keyboard.current.wKey.isPressed ||
            Keyboard.current.upArrowKey.isPressed)
        {
            vertical += 1f;
        }

        inputDirection = Vector2.ClampMagnitude(
            new Vector2(horizontal, vertical),
            1f
        );
    }

    private void HandleMovement()
    {
        Vector3 moveDirection = new Vector3(
            inputDirection.x,
            0f,
            inputDirection.y
        );

        Vector3 targetVelocity = moveDirection * moveSpeed;

        Vector3 currentVelocity = rb.linearVelocity;

        // Keep the current vertical velocity untouched.
        rb.linearVelocity = new Vector3(
            targetVelocity.x,
            currentVelocity.y,
            targetVelocity.z
        );
    }

    private void HandleRotation()
    {
        if (inputDirection.sqrMagnitude < 0.01f)
            return;

        Vector3 moveDirection = new Vector3(
            inputDirection.x,
            0f,
            inputDirection.y
        ).normalized;

        Quaternion targetRotation =
            Quaternion.LookRotation(moveDirection, Vector3.up);

        rb.MoveRotation(
            Quaternion.Slerp(
                rb.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            )
        );
    }

    private void HandleJumpInput()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.spaceKey.wasPressedThisFrame &&
            IsGrounded())
        {
            Jump();
        }
    }

    private void Jump()
    {
        Vector3 velocity = rb.linearVelocity;

        // Remove existing downward velocity so every jump
        // gets a consistent height.
        if (velocity.y < 0f)
        {
            velocity.y = 0f;
            rb.linearVelocity = velocity;
        }

        rb.AddForce(
            Vector3.up * jumpForce,
            ForceMode.Impulse
        );
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(
            transform.position,
            Vector3.down,
            groundCheckDistance + 0.5f,
            groundLayer
        );
    }
}