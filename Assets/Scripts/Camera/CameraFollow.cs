using UnityEngine;


public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Positioning")]
    public Vector3 offset = new Vector3(0f, 6f, -6f);

    [Header("Feel")]
    public float positionSmoothSpeed = 8f;
    public float rotationSmoothSpeed = 8f;

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, positionSmoothSpeed * Time.deltaTime);

        Quaternion desiredRotation = Quaternion.LookRotation(target.position - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSmoothSpeed * Time.deltaTime);
    }
}