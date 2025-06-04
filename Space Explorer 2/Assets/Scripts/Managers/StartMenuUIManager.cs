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
    // leaderboardContent gi? s? là ScoreListContainer b?n ?ã t?o
    public Transform leaderboardContent;
    public GameObject leaderboardEntryPrefab;
    // THÊM DÒNG NÀY: S? l??ng ?i?m t?i ?a mu?n hi?n th?
    public int maxEntriesToShow = 5; // Ví d?: ch? hi?n th? 5 ?i?m cao nh?t

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

        // XÓA DÒNG NÀY:
        // GameStateManager.instance.ResetGameState(); // Dòng này là nguyên nhân làm tên b? m?t ngay l?p t?c!

        // GameStateManager s? ???c reset (tên, ?i?m, th?i gian) t?i các v? trí khác:
        // 1. Khi b?n nh?n "Try Again" ho?c "Back To Main Menu" t? màn hình Game Over (trong EndManager).
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

    // Hàm quan tr?ng nh?t: ?i?n d? li?u vào b?ng x?p h?ng
    private void PopulateLeaderboard()
    {
        // Xóa t?t c? các entry (dòng) c? tr??c khi thêm m?i
        foreach (Transform child in leaderboardContent)
        {
            Destroy(child.gameObject);
        }

        // L?y danh sách ?i?m s? t? LeaderboardManager
        // và GI?I H?N s? l??ng entry theo maxEntriesToShow
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

            // --- Ph?n tìm và gán TextMeshProUGUI v?n gi? nguyên ---
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

            // Gán d? li?u (ch? khi TextMeshProUGUI không null)
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