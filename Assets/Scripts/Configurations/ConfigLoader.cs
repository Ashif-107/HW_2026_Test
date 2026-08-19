using UnityEngine;

public class ConfigLoader : MonoBehaviour
{
    public static ConfigLoader Instance { get; private set; }

    public GameConfig Config { get; private set; }

    private const string ConfigPath = "Config/doofus_diary";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadConfig();
    }

    private void LoadConfig()
    {
        TextAsset configFile = Resources.Load<TextAsset>(ConfigPath);

        if (configFile == null)
        {
            Debug.LogError($"Could not find configuration file at Resources/{ConfigPath}.json");
            return;
        }

        Config = JsonUtility.FromJson<GameConfig>(configFile.text);

        if (Config == null)
        {
            Debug.LogError("Failed to deserialize game configuration.");
            return;
        }

        Debug.Log($"Doofus speed: {Config.player_data.speed}");
        Debug.Log(
            $"Pulpit lifetime: " +
            $"{Config.pulpit_data.min_pulpit_destroy_time} - " +
            $"{Config.pulpit_data.max_pulpit_destroy_time}s"
        );
    }
}