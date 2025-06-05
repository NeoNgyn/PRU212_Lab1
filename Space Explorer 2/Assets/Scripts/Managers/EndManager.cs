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

    // H�M M?I
    private void SavePlayerResult()
    {
        // === B? SUNG DEBUG LOG ?? X�C NH?N KHI H�M N�Y ???C G?I ===

        // ??m b?o c�c instance t?n t?i
        if (GameStateManager.instance == null)
        {
            return;
        }
        if (LeaderboardManager.instance == null)
        {
            return;
        }

        // === B? SUNG KI?M TRA C? TR�NG L?P ?? TR�NH L?U 2 L?N ===
        // ??m b?o GameStateManager c� bi?n public bool HasScoreBeenSavedThisSession;
        if (GameStateManager.instance.HasScoreBeenSavedThisSession)
        {
            
            return; // Tho�t kh?i h�m n?u ?i?m ?� ???c l?u
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

        // Th�m v�o b?ng x?p h?ng
        LeaderboardManager.instance.AddEntry(newEntry);

        // === ??T C? ?? ?�NH D?U R?NG ?I?M ?� ???C L?U TRONG PHI�N N�Y ===
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
            if (nameText != null) // ??m b?o nameText ?� ???c g�n trong Inspector
            {
                string playerNameFromGS = GameStateManager.instance.CurrentPlayerName; // L?y t�n
                nameText.text = $"Player: {playerNameFromGS}"; // G�n t�n
            }

            string minutes = Mathf.Floor(GameStateManager.instance.GetTimeElapsed() / 60).ToString("00");
            string seconds = (GameStateManager.instance.GetTimeElapsed() % 60).ToString("00");
            timerText.text = $"Time: {minutes}:{seconds}.00";
            scoreText.text = $"Score: {(GameStateManager.instance.GetScore()).ToString("N0")}";
        }
        else
        {
            Debug.LogWarning("EndManager: Thi?u tham chi?u ??n timerText, scoreText ho?c GameStateManager. UI c� th? kh�ng ???c c?p nh?t ??y ??.");
        }
    }
}