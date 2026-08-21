using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class DoofusController : MonoBehaviour
{
    [Header("Movement")]
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

    private bool hasFallen = false;
    private float moveSpeed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        ConfigureRigidbody();
    }

    private void Start()
    {
        LoadMovementConfiguration();
    }

    private void Update()
    {
        ReadMovementInput();
        HandleJumpInput();
        CheckFall();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
    }

    private void ConfigureRigidbody()
    {
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        rb.collisionDetectionMode =
            CollisionDetectionMode.Continuous;

        rb.constraints =
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ;
    }

    private void LoadMovementConfiguration()
    {
        if (ConfigLoader.Instance == null)
        {
            Debug.LogError(
                "ConfigLoader instance was not found."
            );

            enabled = false;
            return;
        }

        if (ConfigLoader.Instance.Config == null)
        {
            Debug.LogError(
                "Game configuration has not been loaded."
            );

            enabled = false;
            return;
        }

        moveSpeed =
            ConfigLoader.Instance.Config.player_data.speed;

        Debug.Log(
            $"Doofus movement speed loaded from JSON: {moveSpeed}"
        );
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

        // Left
        if (Keyboard.current.aKey.isPressed ||
            Keyboard.current.leftArrowKey.isPressed)
        {
            horizontal -= 1f;
        }

        // Right
        if (Keyboard.current.dKey.isPressed ||
            Keyboard.current.rightArrowKey.isPressed)
        {
            horizontal += 1f;
        }

        // Backward
        if (Keyboard.current.sKey.isPressed ||
            Keyboard.current.downArrowKey.isPressed)
        {
            vertical -= 1f;
        }

        // Forward
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

        Vector3 targetVelocity =
            moveDirection * moveSpeed;

        Vector3 currentVelocity =
            rb.linearVelocity;


        rb.linearVelocity = new Vector3(
            targetVelocity.x,
            currentVelocity.y,
            targetVelocity.z
        );
    }

    private void HandleRotation()
    {
        if (inputDirection.sqrMagnitude < 0.01f)
        {
            return;
        }

        Vector3 moveDirection = new Vector3(
            inputDirection.x,
            0f,
            inputDirection.y
        ).normalized;

        Quaternion targetRotation =
            Quaternion.LookRotation(
                moveDirection,
                Vector3.up
            );

        Quaternion smoothRotation =
            Quaternion.Slerp(
                rb.rotation,
                targetRotation,
                rotationSpeed *
                Time.fixedDeltaTime
            );

        rb.MoveRotation(smoothRotation);
    }

    private void HandleJumpInput()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame &&
            IsGrounded())
        {
            Jump();
        }
    }


    private void Jump()
    {
        Vector3 velocity = rb.linearVelocity;

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

    private void CheckFall()
    {
        // If Doofus falls below -5 on the Y axis, trigger the Game Over!
        if (transform.position.y < -5f)
        {
            hasFallen = true;
            Debug.Log("Doofus fell off the edge!");
            if (GameOverUI.Instance != null)
            {
                GameOverUI.Instance.ShowGameOver();
            }
        }
    }
}