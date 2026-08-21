using UnityEngine;

public class CharacterSelectionManager : MonoBehaviour
{
    [Header("Preview Models")]
    [Tooltip("Drag all 10 of your 3D models from the hierarchy into this array.")]
    [SerializeField] private GameObject[] characterModels;
    
    [Header("UI Panels")]
    [SerializeField] private GameObject selectionPanel;

    private int currentIndex = 0;

    private void Start()
    {
        // Load the previously saved skin index (defaults to 0)
        currentIndex = PlayerPrefs.GetInt("SelectedSkinIndex", 0);
        
        // Hide the panel by default
        if (selectionPanel != null) selectionPanel.SetActive(false);

        UpdateCharacterPreview();
    }

    public void OpenSelectionPanel()
    {
        if (selectionPanel != null) selectionPanel.SetActive(true);
        UpdateCharacterPreview();
    }

    public void CloseSelectionPanel()
    {
        if (selectionPanel != null) selectionPanel.SetActive(false);
    }

    public void NextCharacter()
    {
        currentIndex++;
        if (currentIndex >= characterModels.Length)
        {
            currentIndex = 0; // Loop back to the first character
        }
        UpdateCharacterPreview();
    }

    public void PreviousCharacter()
    {
        currentIndex--;
        if (currentIndex < 0)
        {
            currentIndex = characterModels.Length - 1; // Loop to the last character
        }
        UpdateCharacterPreview();
    }

    public void SelectCharacter()
    {
        // Save the choice permanently!
        PlayerPrefs.SetInt("SelectedSkinIndex", currentIndex);
        PlayerPrefs.Save();
        
        Debug.Log($"Saved Character Skin Index: {currentIndex}");
        
        CloseSelectionPanel();
    }

    private void UpdateCharacterPreview()
    {
        if (characterModels == null || characterModels.Length == 0) return;

        // Loop through all models and turn ONLY the current one on
        for (int i = 0; i < characterModels.Length; i++)
        {
            if (characterModels[i] != null)
            {
                characterModels[i].SetActive(i == currentIndex);
            }
        }
    }
}
