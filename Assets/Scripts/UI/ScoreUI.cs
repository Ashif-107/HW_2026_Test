using UnityEngine;
using TMPro;
using System.Collections;

public class ScoreUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    
    [Header("Animation Settings")]
    [SerializeField] private float popScale = 1.4f;
    [SerializeField] private float popDuration = 0.3f;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color popColor = Color.yellow;

    private void Start()
    {
        // Subscribe to the score event when the UI is created
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged += UpdateScoreDisplay;
            // Initialize with current score
            scoreText.text = $"SCORE: {ScoreManager.Instance.Score}";
            scoreText.color = defaultColor;
        }
    }

    private void OnDestroy()
    {
        // Always unsubscribe to prevent memory leaks!
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged -= UpdateScoreDisplay;
        }
    }

    private void UpdateScoreDisplay(int newScore)
    {
        scoreText.text = $"SCORE {newScore}";

        StopAllCoroutines();
        StartCoroutine(PopAnimation());
    }

    private IEnumerator PopAnimation()
    {
        Vector3 originalScale = Vector3.one;
        Vector3 targetScale = originalScale * popScale;

        float time = 0;
        
        // Scale up and change color to popColor
        while (time < popDuration / 2)
        {
            time += Time.deltaTime;
            float progress = time / (popDuration / 2);
            
            // Smoothly lerp scale and color
            scoreText.transform.localScale = Vector3.Lerp(originalScale, targetScale, progress);
            scoreText.color = Color.Lerp(defaultColor, popColor, progress);
            
            yield return null; // Wait for next frame
        }

        time = 0;
        
        // Scale back down to normal and fade color back to default
        while (time < popDuration / 2)
        {
            time += Time.deltaTime;
            float progress = time / (popDuration / 2);
            
            scoreText.transform.localScale = Vector3.Lerp(targetScale, originalScale, progress);
            scoreText.color = Color.Lerp(popColor, defaultColor, progress);
            
            yield return null;
        }

        // Ensure we end exactly on the original values
        scoreText.transform.localScale = originalScale;
        scoreText.color = defaultColor;
    }
}
