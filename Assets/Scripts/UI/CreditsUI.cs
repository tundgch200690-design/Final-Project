using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CreditsUI : MonoBehaviour
{
    [Header("Credits Display")]
    [SerializeField] private TextMeshProUGUI creditsText;
    [SerializeField] private ScrollRect creditsScrollRect;
    [SerializeField] private Button backButton;
    [SerializeField] private Button scrollUpButton;
    [SerializeField] private Button scrollDownButton;

    [Header("Credits Content")]
    [TextArea(10, 20)]
    [SerializeField] private string creditsContent = @"
=====================================
           BETRAYAL AT HOUSE
=====================================

DEVELOPMENT TEAM
=====================================
Game Design & Development:
Your Name / Team Name

Programming:
Your Name / Team Name

Art & Graphics:
Your Name / Team Name

Audio & Sound Design:
Your Name / Team Name

Level Design:
Your Name / Team Name

=====================================

SPECIAL THANKS
=====================================
Thanks to all playtesters
and supporters who made this game possible!

=====================================

TECHNOLOGY & TOOLS
=====================================
Unity Engine
TextMesh Pro
URP (Universal Render Pipeline)

=====================================

ASSETS & MUSIC
=====================================
All assets created by the team
Music: [If applicable, credit your music sources]
Sound Effects: [If applicable, credit your SFX sources]

=====================================

Thank you for playing!

=====================================
";

    [Header("Scroll Settings")]
    [SerializeField] private float scrollAmount = 0.1f;
    [SerializeField] private float autoScrollSpeed = 0.5f;
    [SerializeField] private bool autoScroll = false;

    private VerticalLayoutGroup creditsLayoutGroup;

    private void Start()
    {
        SetupButtonListeners();
    }

    public void Initialize()
    {
        DisplayCredits();
    }

    private void SetupButtonListeners()
    {
        if (backButton != null)
            backButton.onClick.AddListener(BackToMenu);

        if (scrollUpButton != null)
            scrollUpButton.onClick.AddListener(ScrollUp);

        if (scrollDownButton != null)
            scrollDownButton.onClick.AddListener(ScrollDown);
    }

    private void DisplayCredits()
    {
        if (creditsText != null)
        {
            creditsText.text = creditsContent;
        }
    }

    private void ScrollUp()
    {
        if (creditsScrollRect != null)
        {
            creditsScrollRect.verticalNormalizedPosition = Mathf.Min(1f, 
                creditsScrollRect.verticalNormalizedPosition + scrollAmount);
        }
    }

    private void ScrollDown()
    {
        if (creditsScrollRect != null)
        {
            creditsScrollRect.verticalNormalizedPosition = Mathf.Max(0f, 
                creditsScrollRect.verticalNormalizedPosition - scrollAmount);
        }
    }

    private void Update()
    {
        if (autoScroll && creditsScrollRect != null)
        {
            creditsScrollRect.verticalNormalizedPosition -= Time.deltaTime * autoScrollSpeed;
            
            if (creditsScrollRect.verticalNormalizedPosition <= 0f)
            {
                autoScroll = false;
            }
        }
    }

    private void BackToMenu()
    {
        autoScroll = false;
        gameObject.SetActive(false);
    }

    public void SetCreditsText(string newCredits)
    {
        creditsContent = newCredits;
        if (gameObject.activeSelf)
        {
            DisplayCredits();
        }
    }

    public void StartAutoScroll()
    {
        autoScroll = true;
        if (creditsScrollRect != null)
            creditsScrollRect.verticalNormalizedPosition = 1f;
    }

    public void StopAutoScroll()
    {
        autoScroll = false;
    }

    private void OnDestroy()
    {
        if (backButton != null)
            backButton.onClick.RemoveListener(BackToMenu);

        if (scrollUpButton != null)
            scrollUpButton.onClick.RemoveListener(ScrollUp);

        if (scrollDownButton != null)
            scrollDownButton.onClick.RemoveListener(ScrollDown);
    }
}
