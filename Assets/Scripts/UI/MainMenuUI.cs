using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Main Menu Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;

    [Header("Menu Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gameModePanelPrefab; // Single/Multiplayer selection
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject creditsPanel;

    [Header("Game Mode Selection")]
    [SerializeField] private Button singlePlayerButton;
    [SerializeField] private Button multiplayerButton;
    [SerializeField] private Button backFromGameModeButton;

    private SettingsMenuUI settingsUI;
    private CreditsUI creditsUI;

    private void Awake()
    {
        // Get references to sub-menus
        if (settingsPanel != null)
        {
            settingsUI = settingsPanel.GetComponent<SettingsMenuUI>();
        }

        if (creditsPanel != null)
        {
            creditsUI = creditsPanel.GetComponent<CreditsUI>();
        }
    }

    private void Start()
    {
        SetupButtonListeners();
        ShowMainMenu();
    }

    private void SetupButtonListeners()
    {
        // Main Menu
        if (playButton != null)
            playButton.onClick.AddListener(ShowGameModeSelection);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(ShowSettings);

        if (creditsButton != null)
            creditsButton.onClick.AddListener(ShowCredits);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);

        // Game Mode Selection
        if (singlePlayerButton != null)
            singlePlayerButton.onClick.AddListener(OnSinglePlayerSelected);

        if (multiplayerButton != null)
            multiplayerButton.onClick.AddListener(OnMultiplayerSelected);

        if (backFromGameModeButton != null)
            backFromGameModeButton.onClick.AddListener(ShowMainMenu);
    }

    private void ShowMainMenu()
    {
        HideAllPanels();
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
    }

    private void ShowGameModeSelection()
    {
        HideAllPanels();
        if (gameModePanelPrefab != null)
            gameModePanelPrefab.SetActive(true);
    }

    private void ShowSettings()
    {
        HideAllPanels();
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            if (settingsUI != null)
                settingsUI.Initialize();
        }
    }

    private void ShowCredits()
    {
        HideAllPanels();
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(true);
            if (creditsUI != null)
                creditsUI.Initialize();
        }
    }

    private void OnSinglePlayerSelected()
    {
        Debug.Log("Single Player selected");
        MainMenuManager.Instance.PlaySinglePlayer();
    }

    private void OnMultiplayerSelected()
    {
        Debug.Log("Multiplayer selected");
        MainMenuManager.Instance.PlayMultiplayer();
    }

    private void HideAllPanels()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        if (gameModePanelPrefab != null)
            gameModePanelPrefab.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (creditsPanel != null)
            creditsPanel.SetActive(false);
    }

    private void QuitGame()
    {
        MainMenuManager.Instance.QuitGame();
    }

    private void OnDestroy()
    {
        RemoveButtonListeners();
    }

    private void RemoveButtonListeners()
    {
        if (playButton != null)
            playButton.onClick.RemoveListener(ShowGameModeSelection);

        if (settingsButton != null)
            settingsButton.onClick.RemoveListener(ShowSettings);

        if (creditsButton != null)
            creditsButton.onClick.RemoveListener(ShowCredits);

        if (quitButton != null)
            quitButton.onClick.RemoveListener(QuitGame);

        if (singlePlayerButton != null)
            singlePlayerButton.onClick.RemoveListener(OnSinglePlayerSelected);

        if (multiplayerButton != null)
            multiplayerButton.onClick.RemoveListener(OnMultiplayerSelected);

        if (backFromGameModeButton != null)
            backFromGameModeButton.onClick.RemoveListener(ShowMainMenu);
    }
}
