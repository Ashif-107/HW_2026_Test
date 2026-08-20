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

        float remaining = GetRemainingTime();

        // 1. Update text to show seconds with 1 decimal place (e.g. "2.4")
        timerText.text = remaining.ToString("F1");

        // 2. Calculate progress from 0 (just spawned) to 1 (about to destroy)
        float progress = elapsedTime / destroyTime;

        // 3. Make the number scale up smoothly as time runs out
        float baseScale = Mathf.Lerp(minTextScale, maxTextScale, progress);

        // 4. Add a "heartbeat" pulse that beats faster and harder as it reaches the end
        float pulseSpeed = 5f + (progress * 15f);
        float pulseIntensity = 0.1f * progress;
        float pulse = Mathf.Sin(Time.time * pulseSpeed) * pulseIntensity;

        float finalScale = baseScale + pulse;

        // --- NEW: Un-squash the text ---
        // Calculate the inverse of the Pulpit's scale so the text doesn't look flattened
        Vector3 parentScale = transform.localScale;
        Vector3 inverseScale = new Vector3(
            1f / parentScale.x,
            1f / parentScale.y,
            1f / parentScale.z
        );

        // Apply the corrected scale
        timerText.transform.localScale = inverseScale * finalScale;

        // 5. Shift the color from White -> Red to increase urgency
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
