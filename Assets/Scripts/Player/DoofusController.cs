using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DoofusController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;

    [Tooltip("How quickly Doofus rotates to face his movement direction.")]
    public float rotationSpeed = 12f;

    private Rigidbody rb;
    private Vector2 inputDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {

        float h = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right
        float v = Input.GetAxisRaw("Vertical");   // W/S or Up/Down
        inputDirection = new Vector2(h, v);
    }

    private void FixedUpdate()
    {
        if (inputDirection.sqrMagnitude < 0.0001f)
            return;

        Vector3 moveDir = new Vector3(inputDirection.x, 0f, inputDirection.y).normalized;

        Vector3 targetPosition = rb.position + moveDir * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(targetPosition);

        Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
    }
}