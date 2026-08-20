using UnityEngine;
using TMPro; 

public class Pulpit : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private TextMeshPro timerText;
    [SerializeField] private float minTextScale = 1f;
    [SerializeField] private float maxTextScale = 2f;
    [SerializeField] private Color startColor = Color.white;
    [SerializeField] private Color endColor = Color.red;

    [Header("Shake Settings")]
    [SerializeField] private float shakeTimeThreshold = 2f;    
    [SerializeField] private float maxShakeIntensity = 0.08f;
    private Vector3 originalPosition;

    private float destroyTime;
    private float elapsedTime;

    private PulpitManager manager;
    private bool playerHasEntered;

    [Header("Fall Settings")]
    [SerializeField] private float fallSpeed = 25f;       
    [SerializeField] private float fallDuration = 2f;
    private bool isFalling = false;


    public void Initialize(PulpitManager pulpitManager)
    {
        manager = pulpitManager;
        originalPosition = transform.position;
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
    }

    private void Update()
    {

        // Fall Logix
        if (isFalling)
        {
            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime, Space.World);

            fallDuration -= Time.deltaTime;
            if (fallDuration <= 0f)
            {
                Expire(); 
            }
            return;
        }

        elapsedTime += Time.deltaTime;

        UpdateTimerUI();

        // Shake Logic
        float remaining = GetRemainingTime();
        if (remaining <= shakeTimeThreshold && remaining > 0)
        {
            float intensityMultiplier = 1f - (remaining / shakeTimeThreshold);
            float currentShake = maxShakeIntensity * intensityMultiplier;

            Vector3 randomOffset = new Vector3(
                Random.Range(-1f, 1f),
                0f,
                Random.Range(-1f, 1f)
            ) * currentShake;

            transform.position = originalPosition + randomOffset;
        }


        if (elapsedTime >= destroyTime)
        {
            StartFalling();
        }
    }

    private void StartFalling()
    {
        isFalling = true;

        if (timerText != null)
        {
            timerText.gameObject.SetActive(false);
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText == null) return;
        timerText.transform.rotation = Quaternion.identity;

        float remaining = GetRemainingTime();

        timerText.text = remaining.ToString("F1");

        float progress = elapsedTime / destroyTime;

        float baseScale = Mathf.Lerp(minTextScale, maxTextScale, progress);

        float pulseSpeed = 5f + (progress * 15f);
        float pulseIntensity = 0.1f * progress;
        float pulse = Mathf.Sin(Time.time * pulseSpeed) * pulseIntensity;

        float finalScale = baseScale + pulse;

        
        Vector3 parentScale = transform.localScale;

        Vector3 inverseScale = new Vector3(
            1f / parentScale.x,
            1f / parentScale.y,
            1f / parentScale.z
        );

        timerText.transform.localScale = inverseScale * finalScale;

        timerText.color = Color.Lerp(startColor, endColor, progress);
    }


    public void PlayerEntered()
    {
        if (playerHasEntered) return;

        playerHasEntered = true;
        if (manager == null)
        {
            Debug.LogError($"{name}: PulpitManager reference is missing.");
            return;
        }
        Debug.Log($"Pulpit.PlayerEntered() → {name}");
        manager.OnPlayerEnteredPulpit(this);
    }

    public float GetRemainingTime()
    {
        return destroyTime - elapsedTime;
    }

    private void Expire()
    {
        if (manager == null)
        {
            Debug.LogError($"{name} has no PulpitManager.");
            Destroy(gameObject);
            return;
        }

        manager.OnPulpitExpired(this);
    }
}
