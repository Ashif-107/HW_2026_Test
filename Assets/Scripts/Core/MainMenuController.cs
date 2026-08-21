using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Needed for Coroutines

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string gameplaySceneName = "SampleScene";

    [Header("Audio")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip chillBgmClip;
    [SerializeField] private AudioClip startGameClip;

    private bool isStarting = false;

    private void Start()
    {
        // Play chill background music as soon as the menu loads!
        if (bgmSource != null && chillBgmClip != null)
        {
            bgmSource.clip = chillBgmClip;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    public void OnPlayButtonClicked()
    {
        // Prevent clicking the button 100 times while the sound is playing
        if (isStarting) return;

        Debug.Log("Play Button Clicked! Loading Game...");

        // If we have a sound effect, wait for it to play before loading the scene
        if (sfxSource != null && startGameClip != null)
        {
            isStarting = true;
            StartCoroutine(PlaySoundAndLoadScene());
        }
        else
        {
            // If no sound is assigned, just load instantly
            SceneManager.LoadScene(gameplaySceneName);
        }
    }

    private IEnumerator PlaySoundAndLoadScene()
    {
        // Optional: Fade out or stop the chill music so it doesn't overlap
        if (bgmSource != null) bgmSource.Stop();

        // Play the start sound
        sfxSource.PlayOneShot(startGameClip);

        // Wait for the sound to finish completely
        yield return new WaitForSeconds(startGameClip.length);

        // Then finally load the game!
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void OnSettingsButtonClicked()
    {
        Debug.Log("Settings Button Clicked!");
    }

    public void OnChangeDoofusButtonClicked()
    {
        Debug.Log("Change Doofus Button Clicked!");
    }

    public void OnExitButtonClicked()
    {
        Debug.Log("Exit Button Clicked! Quitting Game...");
        Application.Quit();
    }
}
