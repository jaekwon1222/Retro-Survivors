using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DifficultySelector : MonoBehaviour
{
    [Header("Buttons – Map 1 Difficulty")]
    public Button map1EasyBtn;
    public Button map1MedBtn;
    public Button map1HardBtn;

    [Header("Buttons – Map 2 Difficulty")]
    public Button map2EasyBtn;
    public Button map2MedBtn;
    public Button map2HardBtn;

    [Header("Buttons – Map 3 Difficulty")]
    public Button map3EasyBtn;
    public Button map3MedBtn;
    public Button map3HardBtn;

    [Header("Map Image Buttons (for green tint)")]
    public Image map1Image;
    public Image map2Image;
    public Image map3Image;

    // MAP 1 ------------------------------------
    public void Map1_Easy() { SelectDifficulty(0, 0); }
    public void Map1_Medium() { SelectDifficulty(0, 1); }
    public void Map1_Hard() { SelectDifficulty(0, 2); }

    // MAP 2 ------------------------------------
    public void Map2_Easy() { SelectDifficulty(1, 0); }
    public void Map2_Medium() { SelectDifficulty(1, 1); }
    public void Map2_Hard() { SelectDifficulty(1, 2); }

    // MAP 3 ------------------------------------
    public void Map3_Easy() { SelectDifficulty(2, 0); }
    public void Map3_Medium() { SelectDifficulty(2, 1); }
    public void Map3_Hard() { SelectDifficulty(2, 2); }


    [Header("Start Button")]
    public Button startButton;

    private int selectedDifficulty = -1; // 0 = easy, 1 = medium, 2 = hard
    private int selectedMap = -1;        // 0 = map1, 1 = map2, 2 = map3

    private Color normalColor = Color.white;
    private Color selectedColor = new Color(0.5f, 1f, 0.5f, 1f); // green tint

    private void Start()
    {
        startButton.interactable = false;
    }

    // ================================
    // DIFFICULTY SELECT
    // ================================
    public void SelectDifficulty(int mapIndex, int difficulty)
    {
        selectedMap = mapIndex;
        selectedDifficulty = difficulty;

        HighlightDifficultyButtons();
        HighlightMapImages();

        CheckReadyToStart();
    }

    // ================================
    // MAP SELECT
    // ================================
    public void SelectMap(int mapIndex)
    {
        selectedMap = mapIndex;

        HighlightMapImages();
        CheckReadyToStart();
    }

    // ================================
    // START GAME
    // ================================
    public void StartGame()
    {
        // Ensure both map and difficulty are selected
        if (selectedMap == -1 || selectedDifficulty == -1)
        {
            Debug.LogWarning("Cannot start: map or difficulty not selected!");
            return;
        }

        // Determine map number explicitly
        int mapNumber = 0; // default
        string mapName = "";

        if (selectedMap == 0) { mapNumber = 0; mapName = "Mall"; }
        else if (selectedMap == 1) { mapNumber = 1; mapName = "Theater"; }
        else if (selectedMap == 2) { mapNumber = 2; mapName = "Map 3"; }

        // Save to PlayerPrefs
        PlayerPrefs.SetInt("SelectedMap", mapNumber);
        PlayerPrefs.SetInt("SelectedDifficulty", selectedDifficulty);

        // Debug info
        string difficultyName = selectedDifficulty == 0 ? "Easy" :
                                selectedDifficulty == 1 ? "Medium" : "Hard";

        Debug.Log($"Starting game with Map: {mapName} ({mapNumber}), Difficulty: {difficultyName} ({selectedDifficulty})");

        PlayerPrefs.SetInt("SelectedCombat", -1);
        // Load the scene
        SceneManager.LoadScene("Scene_Entry 1");
    }



    // ================================
    // HIGHLIGHT HELPERS
    // ================================
    private void HighlightMapImages()
    {
        map1Image.color = (selectedMap == 0) ? selectedColor : normalColor;
        map2Image.color = (selectedMap == 1) ? selectedColor : normalColor;
        map3Image.color = (selectedMap == 2) ? selectedColor : normalColor;
    }

    private void HighlightDifficultyButtons()
    {
        // Reset all button colors
        ResetAllDifficultyButtons();

        // Which group?
        Button easy = null;
        Button med = null;
        Button hard = null;

        if (selectedMap == 0)
        {
            easy = map1EasyBtn;
            med = map1MedBtn;
            hard = map1HardBtn;
        }
        else if (selectedMap == 1)
        {
            easy = map2EasyBtn;
            med = map2MedBtn;
            hard = map2HardBtn;
        }
        else if (selectedMap == 2)
        {
            easy = map3EasyBtn;
            med = map3MedBtn;
            hard = map3HardBtn;
        }

        if (easy == null) return;

        // Apply green tint to the selected difficulty
        if (selectedDifficulty == 0) SetButtonGreen(easy);
        if (selectedDifficulty == 1) SetButtonGreen(med);
        if (selectedDifficulty == 2) SetButtonGreen(hard);
    }

    private void ResetAllDifficultyButtons()
    {
        ResetButtonColor(map1EasyBtn);
        ResetButtonColor(map1MedBtn);
        ResetButtonColor(map1HardBtn);

        ResetButtonColor(map2EasyBtn);
        ResetButtonColor(map2MedBtn);
        ResetButtonColor(map2HardBtn);

        ResetButtonColor(map3EasyBtn);
        ResetButtonColor(map3MedBtn);
        ResetButtonColor(map3HardBtn);
    }

    private void ResetButtonColor(Button b)
    {
        var colors = b.colors;
        colors.normalColor = Color.white;
        b.colors = colors;
    }

    private void SetButtonGreen(Button b)
    {
        var colors = b.colors;
        colors.normalColor = selectedColor;
        b.colors = colors;
    }

    // ================================
    // CHECK IF READY
    // ================================
    private void CheckReadyToStart()
    {
        startButton.interactable = (selectedMap != -1 && selectedDifficulty != -1);
    }
}
