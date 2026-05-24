using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    [SerializeField] private int startingHearts = 3;

    public int Score { get; private set; }
    public int CurrentWave { get; private set; } = 1;
    public int Hearts { get; private set; }
    public int WeaponLevel { get; private set; } = 1;
    public bool IsGameOver { get; private set; }
    public bool IsGameStarted { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Hearts = startingHearts;
        IsGameStarted = false;
    }

    public void StartGame()
    {
        if (IsGameOver || IsGameStarted) return;

        IsGameOver = false;
        IsGameStarted = true;
        CurrentWave = 1;
        Score = 0;
        WeaponLevel = 1;
        Hearts = startingHearts;

        UIManager.Instance?.HideStartScreen();
        UIManager.Instance?.RefreshHUD();
    }

    public void AddScore(int amount)
    {
        if (IsGameOver || !IsGameStarted) return;

        Score += Mathf.Max(0, amount);
        UIManager.Instance?.RefreshHUD();
    }

    public void SetWave(int wave)
    {
        if (IsGameOver || !IsGameStarted) return;

        CurrentWave = Mathf.Max(1, wave);
        WeaponLevel = Mathf.Min(WeaponLevel, GetWeaponLevelCapForWave(CurrentWave));
        UIManager.Instance?.RefreshHUD();
    }

    public void TryUpgradeWeapon()
    {
        if (IsGameOver || !IsGameStarted) return;

        int cap = GetWeaponLevelCapForWave(CurrentWave);
        if (WeaponLevel < cap)
        {
            WeaponLevel++;
            UIManager.Instance?.RefreshHUD();
        }
    }

    public void LoseHeart()
    {
        if (IsGameOver || !IsGameStarted) return;

        Hearts--;
        WeaponLevel = 1;

        if (Hearts <= 0)
        {
            Hearts = 0;
            TriggerGameOver();
        }

        UIManager.Instance?.RefreshHUD();
    }

    private void TriggerGameOver()
    {
        IsGameOver = true;
        UIManager.Instance?.ShowGameOver();
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
    }

    public int GetWeaponLevelCapForWave(int wave)
    {
        if (wave <= 2) return 2;
        if (wave <= 4) return 3;
        if (wave <= 6) return 4;
        return 5;
    }
}
