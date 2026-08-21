using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // Make sure your gameplay scene is exactly named "Gameplay" 
    [SerializeField] private string gameplaySceneName = "SampleScene";

    public void OnPlayButtonClicked()
    {
        Debug.Log("Play Button Clicked! Loading Game...");
        // This tells Unity to load the game scene
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void OnSettingsButtonClicked()
    {
        Debug.Log("Settings Button Clicked! (Placeholder)");
        // Add your settings panel logic here later
    }

    public void OnChangeDoofusButtonClicked()
    {
        Debug.Log("Change Doofus Button Clicked! (Placeholder)");
        // Add your character selection logic here later
    }

    public void OnExitButtonClicked()
    {
        Debug.Log("Exit Button Clicked! Quitting Game...");

        // This quits the built application (Note: doesn't close the Unity Editor window)
        Application.Quit();
    }
}
