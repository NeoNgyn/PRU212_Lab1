using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public class EndManager : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI nameText;
    private void Start()
    {
        SavePlayerResult();
        UpdateUI();
    }

    // HÀM M?I
    private void SavePlayerResult()
    {
        // === B? SUNG DEBUG LOG ?? XÁC NH?N KHI HÀM NÀY ???C G?I ===

        // ??m b?o các instance t?n t?i
        if (GameStateManager.instance == null)
        {
            return;
        }
        if (LeaderboardManager.instance == null)
        {
            return;
        }

        // === B? SUNG KI?M TRA C? TRÙNG L?P ?? TRÁNH L?U 2 L?N ===
        // ??m b?o GameStateManager có bi?n public bool HasScoreBeenSavedThisSession;
        if (GameStateManager.instance.HasScoreBeenSavedThisSession)
        {
            
            return; // Thoát kh?i hàm n?u ?i?m ?ã ???c l?u
        }

        string currentName = GameStateManager.instance.CurrentPlayerName;
        int currentScore = GameStateManager.instance.GetScore();
        float currentTime = GameStateManager.instance.GetTimeElapsed();

        PlayerData newEntry = new PlayerData
        {
            playerName = currentName,
            score = currentScore,
            timePlayed = currentTime
        };

        // Thêm vào b?ng x?p h?ng
        LeaderboardManager.instance.AddEntry(newEntry);

        // === ??T C? ?? ?ÁNH D?U R?NG ?I?M ?Ã ???C L?U TRONG PHIÊN NÀY ===
        GameStateManager.instance.HasScoreBeenSavedThisSession = true;
    }

    public void TryAgain()
    {
        // Ensure GameStateManager exists before resetting
        if (GameStateManager.instance != null)
        {
            GameStateManager.instance.ResetGameStateTRyAgian();
        }
        SceneManager.LoadScene("GameScene");
    }

    public void BackToMainMenu()
    {
        // Ensure GameStateManager exists before resetting
        if (GameStateManager.instance != null)
        {
            GameStateManager.instance.ResetGameState();
        }
        SceneManager.LoadScene("StartMenuScene");
    }

    private void UpdateUI()
    {
        if (timerText != null && scoreText != null && GameStateManager.instance != null)
        {
            if (nameText != null) // ??m b?o nameText ?ã ???c gán trong Inspector
            {
                string playerNameFromGS = GameStateManager.instance.CurrentPlayerName; // L?y tên
                nameText.text = $"Player: {playerNameFromGS}"; // Gán tên
            }

            string minutes = Mathf.Floor(GameStateManager.instance.GetTimeElapsed() / 60).ToString("00");
            string seconds = (GameStateManager.instance.GetTimeElapsed() % 60).ToString("00");
            timerText.text = $"Time: {minutes}:{seconds}.00";
            scoreText.text = $"Score: {(GameStateManager.instance.GetScore()).ToString("N0")}";
        }
        else
        {
            Debug.LogWarning("EndManager: Thi?u tham chi?u ??n timerText, scoreText ho?c GameStateManager. UI có th? không ???c c?p nh?t ??y ??.");
        }
    }
}