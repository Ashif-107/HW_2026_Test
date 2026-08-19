using UnityEngine;

public class Pulpit : MonoBehaviour
{
    private float destroyTime;
    private float elapsedTime;

    private void Start()
    {
        GameConfig config = ConfigLoader.Instance.Config;

        destroyTime = Random.Range(
            config.pulpit_data.min_pulpit_destroy_time,
            config.pulpit_data.max_pulpit_destroy_time
        );

        Debug.Log($"{name} will disappear in {destroyTime:F2}s");
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= destroyTime)
        {
            Destroy(gameObject);
        }
    }
}