using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private Text scoreText;
    [SerializeField] private Text waveText;
    [SerializeField] private Text heartsText;
    [SerializeField] private Text weaponText;
    [Header("Start Screen")]
    [SerializeField] private GameObject startScreenPanel;
    [SerializeField] private Text titleText;
    [SerializeField] private Text subtitleText;

    [Header("Game Over")]
    [SerializeField] private GameObject gameOverPanel;

    private bool hasWarnedMissingScoreText;
    private bool hasWarnedMissingWaveText;
    private bool hasWarnedMissingHeartsText;
    private bool hasWarnedMissingWeaponText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        ValidateRequiredReferences();
    }

    private void Start()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        ShowStartScreen();

        if (titleText != null)
        {
            titleText.text = "Kawaii Starfall";
        }

        if (subtitleText != null)
        {
            subtitleText.text = "A neon arcade shooter.";
        }

        RefreshHUD();
    }

    public void ShowStartScreen()
    {
        if (startScreenPanel != null)
        {
            startScreenPanel.SetActive(true);
            return;
        }

        Debug.LogWarning("UIManager: Start Screen panel is not assigned. Start UI will not be shown.");
    }

    public void HideStartScreen()
    {
        if (startScreenPanel != null)
        {
            startScreenPanel.SetActive(false);
            return;
        }

        Debug.LogWarning("UIManager: Start Screen panel is not assigned. Nothing to hide.");
    }

    public void RefreshHUD()
    {
        if (GameManager.Instance == null) return;

        if (scoreText != null)
        {
            scoreText.text = $"Score: {GameManager.Instance.Score}";
        }
        else if (!hasWarnedMissingScoreText)
        {
            hasWarnedMissingScoreText = true;
            Debug.LogWarning("UIManager: scoreText is not assigned in the Inspector.");
        }

        if (waveText != null)
        {
            waveText.text = $"Wave: {GameManager.Instance.CurrentWave}";
        }
        else if (!hasWarnedMissingWaveText)
        {
            hasWarnedMissingWaveText = true;
            Debug.LogWarning("UIManager: waveText is not assigned in the Inspector.");
        }

        if (heartsText != null)
        {
            heartsText.text = $"Hearts: {GameManager.Instance.Hearts}";
        }
        else if (!hasWarnedMissingHeartsText)
        {
            hasWarnedMissingHeartsText = true;
            Debug.LogWarning("UIManager: heartsText is not assigned in the Inspector.");
        }

        if (weaponText != null)
        {
            weaponText.text = $"Weapon Lv: {GameManager.Instance.WeaponLevel}";
        }
        else if (!hasWarnedMissingWeaponText)
        {
            hasWarnedMissingWeaponText = true;
            Debug.LogWarning("UIManager: weaponText is not assigned in the Inspector.");
        }
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            return;
        }

        Debug.LogWarning("UIManager: Game Over panel is not assigned. Game Over UI will not be shown.");
    }

    public void OnRestartPressed()
    {
        GameManager.Instance?.RestartGame();
    }

    public void OnStartGamePressed()
    {
        GameManager.Instance?.StartGame();
    }

    private void ValidateRequiredReferences()
    {
        if (startScreenPanel == null)
        {
            Debug.LogWarning("UIManager: startScreenPanel is not assigned in the Inspector.");
        }

        if (titleText == null)
        {
            Debug.LogWarning("UIManager: titleText is not assigned in the Inspector.");
        }

        if (subtitleText == null)
        {
            Debug.LogWarning("UIManager: subtitleText is not assigned in the Inspector.");
        }

        if (gameOverPanel == null)
        {
            Debug.LogWarning("UIManager: gameOverPanel is not assigned in the Inspector.");
        }
    }
}
