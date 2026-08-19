using UnityEngine;

public class PlayerPulpitDetector : MonoBehaviour
{
    private Pulpit pulpit;

    private void Awake()
    {
        pulpit = GetComponentInParent<Pulpit>();

        if (pulpit == null)
        {
            Debug.LogError(
                "PlayerPulpitDetector could not find a Pulpit in its parent."
            );
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(
            $"Trigger entered by: {other.gameObject.name}"
        );

        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (pulpit == null)
        {
            return;
        }

        Debug.Log(
            $"PLAYER ENTERED PULPIT: {pulpit.name}"
        );

        pulpit.PlayerEntered();
    }
}