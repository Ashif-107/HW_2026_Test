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

    private Pulpit currentPulpit;
    private Pulpit nextPulpit;

    private float spawnTimer;

    private bool gameOver;

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

    private void HandleNextPulpitSpawn()
    {
        // We already have a next Pulpit.
        if (nextPulpit != null)
        {
            return;
        }

        // We don't have a current Pulpit.
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
        if (pulpitPrefab == null)
        {
            Debug.LogError(
                "Pulpit prefab is not assigned."
            );

            return null;
        }

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

    public void OnPlayerEnteredPulpit(
        Pulpit pulpit)
    {
        if (gameOver)
        {
            return;
        }

        if (pulpit == null)
        {
            return;
        }

        // Player is already on this Pulpit.
        if (pulpit == currentPulpit)
        {
            return;
        }

        // Player has entered a Pulpit that isn't
        // the expected next Pulpit.
        if (pulpit != nextPulpit)
        {
            return;
        }

        Debug.Log(
            $"Player reached {pulpit.name}"
        );

        // Old current Pulpit.
        Pulpit oldCurrent = currentPulpit;

        // Promote next → current.
        currentPulpit = nextPulpit;

        nextPulpit = null;

        spawnTimer = 0f;

        Debug.Log(
            $"Current Pulpit is now {currentPulpit.name}"
        );

        /*
         * We don't immediately destroy oldCurrent.
         *
         * It will naturally expire according to
         * its own lifetime.
         *
         * Score will be added here later.
         */
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

            spawnTimer = 0f;

            Destroy(pulpit.gameObject);

            return;
        }

        Destroy(pulpit.gameObject);
    }
}