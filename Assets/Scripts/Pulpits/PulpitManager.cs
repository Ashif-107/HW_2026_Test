using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Biome
{
    public string biomeName;
    public GameObject[] prefabs;
}

public class PulpitManager : MonoBehaviour
{
    [Header("Pulpit")]
    [SerializeField] private Biome[] biomes;
    [SerializeField] private int minPulpitsPerBiome = 4;
    [SerializeField] private int maxPulpitsPerBiome = 7;
    [SerializeField] private float pulpitSize = 9f;

    [Header("Spawn")]
    [SerializeField] private Transform startingPosition;

    private Pulpit currentPulpit;
    private Pulpit nextPulpit;

    private bool gameOver;

    private Biome currentBiome;
    private int pulpitsLeftInBiome = 0;

    private readonly List<Pulpit> activePulpits = new();


    private void Start()
    {
        SpawnInitialPulpit();
    }

    private void Update()
    {
        if (gameOver)
        {
            return;
        }

        HandleNextPulpitSpawn();
    }

    private float spawnTimer;

    private void HandleNextPulpitSpawn()
    {
        if (nextPulpit != null)
        {
            return;
        }

        if (currentPulpit == null)
        {
            return;
        }

        GameConfig config = ConfigLoader.Instance.Config;

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

        Pulpit pulpit = SpawnPulpit(position);

        if (pulpit == null)
        {
            return;
        }

        currentPulpit = pulpit;

        Debug.Log(
            $"Current Pulpit: {currentPulpit.name}"
        );
    }

    private void SpawnNextPulpit()
    {
        if (currentPulpit == null)
        {
            return;
        }

        Vector3 spawnPosition =
            GetValidAdjacentPosition(
                currentPulpit.transform.position
            );

        Pulpit pulpit =
            SpawnPulpit(spawnPosition);

        if (pulpit == null)
        {
            return;
        }

        nextPulpit = pulpit;

        Debug.Log(
            $"Next Pulpit spawned: {nextPulpit.name}"
        );
    }

    private Pulpit SpawnPulpit(Vector3 position)
    {
        if (biomes == null || biomes.Length == 0)
        {
            Debug.LogError("No Biomes defined in PulpitManager.");
            return null;
        }

        if (currentBiome == null || pulpitsLeftInBiome <= 0)
        {
            currentBiome = biomes[Random.Range(0, biomes.Length)];
            pulpitsLeftInBiome = Random.Range(minPulpitsPerBiome, maxPulpitsPerBiome + 1);
            Debug.Log($"Entered Biome: {currentBiome.biomeName}. It will last for {pulpitsLeftInBiome} pulpits.");
        }

        if (currentBiome.prefabs == null || currentBiome.prefabs.Length == 0)
        {
            Debug.LogError($"Biome '{currentBiome.biomeName}' has no prefabs assigned.");
            return null;
        }

        GameObject prefabToSpawn = currentBiome.prefabs[Random.Range(0, currentBiome.prefabs.Length)];

        pulpitsLeftInBiome--;


        float randomYRotation = Random.Range(0, 4) * 90f;
        Quaternion rotation = Quaternion.Euler(0f, randomYRotation, 0f);

        GameObject pulpitObject = Instantiate(
            prefabToSpawn,
            position,
            rotation
        );

        Pulpit pulpit =
            pulpitObject.GetComponent<Pulpit>();

        if (pulpit == null)
        {
            Debug.LogError(
                "Pulpit prefab is missing Pulpit.cs."
            );

            Destroy(pulpitObject);

            return null;
        }

        activePulpits.Add(pulpit);

        pulpit.Initialize(this);

        return pulpit;
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

        Debug.LogWarning(
            "No valid adjacent Pulpit position found."
        );

        return currentPosition +
               Vector3.forward * pulpitSize;
    }

    private bool IsPositionAvailable(
        Vector3 position)
    {
        foreach (Pulpit pulpit in activePulpits)
        {
            if (pulpit == null)
            {
                continue;
            }

            if (Vector3.Distance(
                    pulpit.transform.position,
                    position
                ) < 0.1f)
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

    public void OnPlayerEnteredPulpit(Pulpit pulpit)
    {
        if (gameOver)
        {
            return;
        }

        if (pulpit == null)
        {
            return;
        }

        if (pulpit == currentPulpit)
        {
            return;
        }

        if (pulpit != nextPulpit)
        {
            return;
        }

        Debug.Log(
            $"Player reached {pulpit.name}"
        );

        currentPulpit = nextPulpit;

        nextPulpit = null;

        // Start a fresh 2.5 second countdown
        // for the next Pulpit.
        spawnTimer = 0f;

        Debug.Log(
            $"Current Pulpit is now {currentPulpit.name}"
        );
    }

    public void OnPulpitExpired(Pulpit pulpit)
    {
        if (pulpit == null)
        {
            return;
        }

        activePulpits.Remove(pulpit);

        // Current Pulpit disappeared.
        if (pulpit == currentPulpit)
        {
            Debug.Log(
                "CURRENT PULPIT EXPIRED - GAME OVER"
            );

            gameOver = true;

            currentPulpit = null;

            if (nextPulpit != null)
            {
                activePulpits.Remove(nextPulpit);

                Destroy(nextPulpit.gameObject);

                nextPulpit = null;
            }

            Destroy(pulpit.gameObject);

            return;
        }

        // Next Pulpit disappeared before
        // the player reached it.
        if (pulpit == nextPulpit)
        {
            Debug.Log(
                "Next Pulpit expired before player reached it."
            );

            nextPulpit = null;

            Destroy(pulpit.gameObject);

            return;
        }

        Destroy(pulpit.gameObject);
    }
}