using System.Collections.Generic;
using UnityEngine;

public class PulpitManager : MonoBehaviour
{
    [Header("Pulpit")]
    [SerializeField] private GameObject pulpitPrefab;
    [SerializeField] private float pulpitSize = 9f;

    [Header("Spawn")]
    [SerializeField] private Transform startingPosition;

    private readonly List<Pulpit> activePulpits = new();

    private float spawnTimer;

    private void Start()
    {
        SpawnInitialPulpit();
    }

    private void Update()
    {
        HandleSpawning();
    }

    private void HandleSpawning()
    {
        GameConfig config = ConfigLoader.Instance.Config;

        // We already have two Pulpits.
        if (activePulpits.Count >= 2)
        {
            return;
        }

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= config.pulpit_data.pulpit_spawn_time)
        {
            SpawnNextPulpit();

            spawnTimer = 0f;
        }
    }

    private void SpawnInitialPulpit()
    {
        Vector3 position = startingPosition != null
            ? startingPosition.position
            : Vector3.zero;

        SpawnPulpit(position);
    }

    private void SpawnNextPulpit()
    {
        if (activePulpits.Count == 0)
        {
            SpawnInitialPulpit();
            return;
        }

        Pulpit referencePulpit = activePulpits[^1];

        Vector3 spawnPosition =
            GetValidAdjacentPosition(
                referencePulpit.transform.position
            );

        SpawnPulpit(spawnPosition);
    }

    private void SpawnPulpit(Vector3 position)
    {
        GameObject pulpitObject = Instantiate(
            pulpitPrefab,
            position,
            Quaternion.identity
        );

        Pulpit pulpit =
            pulpitObject.GetComponent<Pulpit>();

        if (pulpit == null)
        {
            Debug.LogError(
                "Pulpit prefab is missing the Pulpit component."
            );

            Destroy(pulpitObject);
            return;
        }

        activePulpits.Add(pulpit);

        pulpit.Initialize(this);

        Debug.Log(
            $"Pulpit spawned at {pulpit.transform.position}"
        );
    }

    private Vector3 GetValidAdjacentPosition(
        Vector3 currentPosition)
    {
        Vector3[] directions =
        {
            Vector3.forward,
            Vector3.back,
            Vector3.left,
            Vector3.right
        };

        // Randomize the order of the four directions.
        ShuffleDirections(directions);

        foreach (Vector3 direction in directions)
        {
            Vector3 candidatePosition =
                currentPosition +
                direction * pulpitSize;

            if (IsPositionAvailable(candidatePosition))
            {
                return candidatePosition;
            }
        }

        // This should rarely happen.
        Debug.LogWarning(
            "Could not find a valid adjacent Pulpit position."
        );

        return currentPosition + Vector3.forward * pulpitSize;
    }

    private bool IsPositionAvailable(
        Vector3 position)
    {
        foreach (Pulpit pulpit in activePulpits)
        {
            if (Vector3.Distance(
                    pulpit.transform.position,
                    position) < 0.1f)
            {
                return false;
            }
        }

        return true;
    }

    private void ShuffleDirections(
        Vector3[] directions)
    {
        for (int i = directions.Length - 1; i > 0; i--)
        {
            int randomIndex =
                Random.Range(0, i + 1);

            (
                directions[i],
                directions[randomIndex]
            ) =
            (
                directions[randomIndex],
                directions[i]
            );
        }
    }

    public void OnPulpitExpired(Pulpit pulpit)
    {
        if (pulpit == null)
        {
            return;
        }

        if (!activePulpits.Remove(pulpit))
        {
            return;
        }

        Debug.Log(
            $"Pulpit expired: {pulpit.gameObject.name}"
        );

        Destroy(pulpit.gameObject);
    }
}