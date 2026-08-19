using UnityEngine;

public class Pulpit : MonoBehaviour
{
    private float destroyTime;
    private float elapsedTime;

    private PulpitManager manager;

    public void Initialize(PulpitManager pulpitManager)
    {
        manager = pulpitManager;

        GameConfig config =
            ConfigLoader.Instance.Config;

        destroyTime = Random.Range(
            config.pulpit_data.min_pulpit_destroy_time,
            config.pulpit_data.max_pulpit_destroy_time
        );

        elapsedTime = 0f;

        Debug.Log(
            $"{gameObject.name} lifetime: " +
            $"{destroyTime:F2}s"
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

    private void Expire()
    {
        if (manager == null)
        {
            Debug.LogError(
                $"{gameObject.name} has no PulpitManager."
            );

            Destroy(gameObject);
            return;
        }

        manager.OnPulpitExpired(this);
    }
}