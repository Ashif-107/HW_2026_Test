using UnityEngine;

public class Pulpit : MonoBehaviour
{
    private float destroyTime;
    private float elapsedTime;

    private PulpitManager manager;

    private bool playerHasEntered;

    public void Initialize(PulpitManager pulpitManager)
    {
        manager = pulpitManager;

        LoadLifetime();
    }

    private void LoadLifetime()
    {
        GameConfig config = ConfigLoader.Instance.Config;

        destroyTime = Random.Range(
            config.pulpit_data.min_pulpit_destroy_time,
            config.pulpit_data.max_pulpit_destroy_time
        );

        elapsedTime = 0f;

        Debug.Log(
            $"{name} lifetime: {destroyTime:F2}s"
        );
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= destroyTime)
        {
            Expire();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (playerHasEntered)
        {
            return;
        }

        playerHasEntered = true;

        manager.OnPlayerEnteredPulpit(this);
    }

    private void Expire()
    {
        if (manager == null)
        {
            Debug.LogError(
                $"{name} has no PulpitManager."
            );

            Destroy(gameObject);
            return;
        }

        manager.OnPulpitExpired(this);
    }
}