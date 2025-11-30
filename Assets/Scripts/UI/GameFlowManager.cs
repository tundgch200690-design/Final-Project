using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance { get; private set; }

    [Header("Scene Names")]
    [SerializeField] private string characterSelectionSceneName = "CharacterSelection";
    [SerializeField] private string gamePlaySceneName = "GameScene";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadCharacterSelection()
    {
        SceneManager.LoadScene(characterSelectionSceneName);
    }

    public void LoadGamePlay()
    {
        // Validate that players have been selected
        if (CharacterSelectionManager.Instance != null)
        {
            CharacterSelectionManager.Instance.StartGame();
            SceneManager.LoadScene(gamePlaySceneName);
        }
        else
        {
            Debug.LogError("CharacterSelectionManager not found!");
        }
    }

    public void ReturnToCharacterSelection()
    {
        if (CharacterSelectionManager.Instance != null)
        {
            CharacterSelectionManager.Instance.ClearSelection();
        }
        SceneManager.LoadScene(characterSelectionSceneName);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
