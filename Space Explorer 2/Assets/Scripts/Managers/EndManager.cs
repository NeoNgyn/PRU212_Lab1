using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EndManager : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;

    private void Start()
    {
        UpdateUI();
    }

    public void TryAgain()
    {
        // Ensure GameStateManager exists before resetting
        if (GameStateManager.instance != null)
        {
            GameStateManager.instance.ResetGameState();
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
            string minutes = Mathf.Floor(GameStateManager.instance.GetTimeElapsed() / 60).ToString("00");
            string seconds = (GameStateManager.instance.GetTimeElapsed() % 60).ToString("00");
            timerText.text = $"Time: {minutes}:{seconds}.00";
            scoreText.text = $"Score: {GameStateManager.instance.GetScore():D5}";
        }
        else
        {
            Debug.LogWarning("EndManager: Missing references to timerText, scoreText and name");
        }
    }
}