using UnityEngine;

public class CharacterSkinLoader : MonoBehaviour
{
    [Header("Skin Models")]
    [Tooltip("Drag all 10 of your 3D models from the hierarchy into this array. MUST be in the exact same order as the Main Menu!")]
    [SerializeField] private GameObject[] characterModels;

    private void Awake()
    {
        // 1. Read the saved skin index (defaults to 0 if they haven't picked one yet)
        int selectedSkinIndex = PlayerPrefs.GetInt("SelectedSkinIndex", 0);

        // Safety check just in case!
        if (characterModels == null || characterModels.Length == 0)
        {
            Debug.LogError("No character models assigned to CharacterSkinLoader!");
            return;
        }

        // 2. Loop through all models and turn ONLY the saved one on
        for (int i = 0; i < characterModels.Length; i++)
        {
            if (characterModels[i] != null)
            {
                // If this is the chosen index, set it to true. Otherwise, false.
                characterModels[i].SetActive(i == selectedSkinIndex);
            }
        }
        
        Debug.Log($"Loaded Character Skin Index: {selectedSkinIndex}");
    }
}
