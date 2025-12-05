using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DropDownEx : MonoBehaviour
{
    [Header("Dropdown")]
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private Image targetImage;
    [SerializeField] private Sprite option0Sprite; // Mall
    [SerializeField] private Sprite option1Sprite; // Theater

    [Header("Difficulty Buttons")]
    public Button easyBtn;
    public Button medBtn;
    public Button hardBtn;
    public Button veryHardBtn; // New button for Very Hard

    [Header("Start Button")]
    public Button startButton;

    private int selectedMap = -1;        // 0 = Mall, 1 = Theater
    private int selectedDifficulty = -1; // 0 = Easy, 1 = Medium, 2 = Hard, 3 = Very Hard

    private Color normalColor = Color.white;
    private Color selectedColor = new Color(0.5f, 1f, 0.5f, 1f);

    private void Start()
    {
        // Initialize dropdown
        dropdown.value = 0;
        dropdown.RefreshShownValue();
        OnDropdownChanged(0);
        dropdown.onValueChanged.AddListener(OnDropdownChanged);

        // Setup difficulty buttons
        if (veryHardBtn != null) veryHardBtn.onClick.AddListener(() => SelectDifficulty(3)); // very hard = 3
        if (startButton != null) startButton.onClick.AddListener(StartGame);

        startButton.interactable = false;
        startButton.onClick.AddListener(StartGame);
    }

    // ----------------------------
    // Dropdown selection
    // ----------------------------
    public void OnDropdownChanged(int index)
    {
        selectedMap = index;

        // Change image
        switch (index)
        {
            case 0: targetImage.sprite = option0Sprite; break; // Mall
            case 1: targetImage.sprite = option1Sprite; break; // Theater
        }

        Debug.Log($"Map selected: {(selectedMap == 0 ? "Mall" : "Theater")} ({selectedMap})");

        CheckReadyToStart();
    }

    // ----------------------------
    // Difficulty selection
    // ----------------------------
    public void SelectDifficulty(int difficulty)
    {
        selectedDifficulty = difficulty;
        UpdateDifficultyButtonColors();
        CheckReadyToStart();

        string difficultyName = difficulty == 0 ? "Easy" :
                                difficulty == 1 ? "Medium" :
                                difficulty == 2 ? "Hard" : "Endless";

        Debug.Log($"Difficulty selected: {difficultyName} ({selectedDifficulty})");
    }

    private void UpdateDifficultyButtonColors()
    {
        ResetButtonColors();

        switch (selectedDifficulty)
        {
            case 0: if (easyBtn != null) easyBtn.image.color = selectedColor; break;
            case 1: if (medBtn != null) medBtn.image.color = selectedColor; break;
            case 2: if (hardBtn != null) hardBtn.image.color = selectedColor; break;
            case 3: if (veryHardBtn != null) veryHardBtn.image.color = selectedColor; break;
        }
    }

    private void ResetButtonColors()
    {
        if (easyBtn != null) easyBtn.image.color = normalColor;
        if (medBtn != null) medBtn.image.color = normalColor;
        if (hardBtn != null) hardBtn.image.color = normalColor;
        if (veryHardBtn != null) veryHardBtn.image.color = normalColor;
    }

    // ----------------------------
    // Check if ready
    // ----------------------------
    private void CheckReadyToStart()
    {
        startButton.interactable = (selectedMap != -1 && selectedDifficulty != -1);
    }

    // ----------------------------
    // Start game
    // ----------------------------
    private void StartGame()
    {
        if (selectedMap == -1 || selectedDifficulty == -1)
        {
            Debug.LogWarning("Cannot start: Map or difficulty not selected!");
            return;
        }

        int mapNumber = selectedMap; // already 0 or 1
        string mapName = selectedMap == 0 ? "Mall" : "Theater";

        string difficultyName = selectedDifficulty == 0 ? "Easy" :
                                selectedDifficulty == 1 ? "Medium" :
                                selectedDifficulty == 2 ? "Hard" : "Endless";

        Debug.Log($"Starting game with Map: {mapName} ({mapNumber}), Difficulty: {difficultyName} ({selectedDifficulty})");

        // Save map and difficulty
        PlayerPrefs.SetInt("SelectedMap", mapNumber);
        PlayerPrefs.SetInt("SelectedDifficulty", selectedDifficulty);

        var ui = FindObjectOfType<UIManager>();
        if (ui) Destroy(ui.gameObject);

        // Load scene
        SceneManager.LoadScene("Scene_Entry 1");
    }
}
