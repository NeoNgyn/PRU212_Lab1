using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq; // C?n thi?t cho .Take()


public class StartMenuUIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject enterNamePanel;
    public GameObject leaderboardPanel;

    [Header("Name Input")]
    public TMP_InputField nameInputField;
    public Button continueButton;
    public TextMeshProUGUI errorMessageText;

    [Header("Leaderboard")]
    public Button leaderboardButton;
    // leaderboardContent gi? s? l� ScoreListContainer b?n ?� t?o
    public Transform leaderboardContent;
    public GameObject leaderboardEntryPrefab;
    // TH�M D�NG N�Y: S? l??ng ?i?m t?i ?a mu?n hi?n th?
    public int maxEntriesToShow = 5; // V� d?: ch? hi?n th? 5 ?i?m cao nh?t

    void Start()
    {
        enterNamePanel.SetActive(false);
        leaderboardPanel.SetActive(false);
        if (errorMessageText != null) errorMessageText.gameObject.SetActive(false);
    }

    public void OnPlayButtonClicked()
    {
        enterNamePanel.SetActive(true);
        if (errorMessageText != null) errorMessageText.gameObject.SetActive(false);
        nameInputField.text = "";
    }

    // Trong StartMenuUIManager.cs
    public void OnContinueButtonClicked()
    {
        string playerName = nameInputField.text;

        if (string.IsNullOrWhiteSpace(playerName))
        {
            if (errorMessageText != null)
            {
                errorMessageText.text = "NAME IS REQUIRED";
                errorMessageText.gameObject.SetActive(true);
            }
            return;
        }

        GameStateManager.instance.CurrentPlayerName = playerName;        

        // X�A D�NG N�Y:
        // GameStateManager.instance.ResetGameState(); // D�ng n�y l� nguy�n nh�n l�m t�n b? m?t ngay l?p t?c!

        // GameStateManager s? ???c reset (t�n, ?i?m, th?i gian) t?i c�c v? tr� kh�c:
        // 1. Khi b?n nh?n "Try Again" ho?c "Back To Main Menu" t? m�n h�nh Game Over (trong EndManager).
        // 2. Khi b?n nh?n "Quit Game" t? Pause Menu (trong GameManager).

        SceneManager.LoadScene("GameScene"); // T?i Scene Game
    }

    public void OnLeaderboardButtonClicked()
    {
        PopulateLeaderboard();
        leaderboardPanel.SetActive(true);
    }

    public void CloseLeaderboardPanel()
    {
        leaderboardPanel.SetActive(false);
    }

    // H�m quan tr?ng nh?t: ?i?n d? li?u v�o b?ng x?p h?ng
    private void PopulateLeaderboard()
    {
        // X�a t?t c? c�c entry (d�ng) c? tr??c khi th�m m?i
        foreach (Transform child in leaderboardContent)
        {
            Destroy(child.gameObject);
        }

        // L?y danh s�ch ?i?m s? t? LeaderboardManager
        // v� GI?I H?N s? l??ng entry theo maxEntriesToShow
        List<PlayerData> entries = LeaderboardManager.instance.GetLeaderboardEntries()
                                  .Take(maxEntriesToShow) // Ch? l?y s? l??ng entry mong mu?n
                                  .ToList();

        for (int i = 0; i < entries.Count; i++)
        {
            if (leaderboardEntryPrefab == null)
            {
                return;
            }
            if (leaderboardContent == null)
            {
                return;
            }

            GameObject entryObject = Instantiate(leaderboardEntryPrefab, leaderboardContent);

            if (entryObject == null)
            {
                continue;
            }

            // --- Ph?n t�m v� g�n TextMeshProUGUI v?n gi? nguy�n ---
            TextMeshProUGUI rankText = null;
            TextMeshProUGUI nameText = null;
            TextMeshProUGUI scoreText = null;
            TextMeshProUGUI timeText = null;

            Transform foundTransform = entryObject.transform.Find("RankText");
            if (foundTransform != null) { rankText = foundTransform.GetComponent<TextMeshProUGUI>(); }

            foundTransform = entryObject.transform.Find("NameText");
            if (foundTransform != null) { nameText = foundTransform.GetComponent<TextMeshProUGUI>(); }

            foundTransform = entryObject.transform.Find("ScoreText");
            if (foundTransform != null) { scoreText = foundTransform.GetComponent<TextMeshProUGUI>(); }          

            foundTransform = entryObject.transform.Find("TimeText");
            if (foundTransform != null) { timeText = foundTransform.GetComponent<TextMeshProUGUI>(); }          

            // G�n d? li?u (ch? khi TextMeshProUGUI kh�ng null)
            if (rankText != null) rankText.text = (i + 1).ToString();
            if (nameText != null) nameText.text = entries[i].playerName;
            if (scoreText != null) scoreText.text = entries[i].score.ToString("N0");

            float time = entries[i].timePlayed;
            string minutes = Mathf.Floor(time / 60).ToString("00");
            string seconds = (time % 60).ToString("00");
            if (timeText != null) timeText.text = $"{minutes}:{seconds}";
        }
    }
}