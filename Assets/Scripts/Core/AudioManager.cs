using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip bgmClip;
    [SerializeField] private AudioClip scorePopClip;

    private void Start()
    {
        // Start playing the background music on loop
        if (bgmSource != null && bgmClip != null)
        {
            bgmSource.clip = bgmClip;
            bgmSource.loop = true;
            bgmSource.Play();
        }

        // Listen for when the score changes!
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged += PlayScoreSound;
        }
    }

    private void OnDestroy()
    {
        // Always unsubscribe to prevent errors when scene reloads
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged -= PlayScoreSound;
        }
    }

    private void PlayScoreSound(int newScore)
    {
        // Play the pop sound effect once without interrupting other sounds
        if (sfxSource != null && scorePopClip != null)
        {
            sfxSource.PlayOneShot(scorePopClip);
        }
    }
}
