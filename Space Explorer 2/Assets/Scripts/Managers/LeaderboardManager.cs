// File: Assets/Scripts/Managers/LeaderboardManager.cs
using UnityEngine;
using System.Collections.Generic; // C?n thi?t cho List
using System.Linq; // C?n thi?t cho OrderByDescending (?? s?p x?p)

public class LeaderboardManager : MonoBehaviour
{
    // Thi?t k? Singleton: ??m b?o ch? có m?t th? hi?n c?a LeaderboardManager t?n t?i.
    public static LeaderboardManager instance;

    private Leaderboard leaderboardData; // ??i t??ng ch?a toàn b? d? li?u b?ng x?p h?ng
    private const string LeaderboardSaveKey = "LeaderboardData"; // Khóa dùng ?? l?u vào PlayerPrefs

    private void Awake()
    {
        // Ki?m tra xem ?ã có instance nào c?a LeaderboardManager ch?a.
        if (instance == null)
        {
            instance = this; // Gán instance này làm th? hi?n duy nh?t.
            // Gi? cho GameObject này không b? h?y khi chuy?n ??i gi?a các Scene.
            DontDestroyOnLoad(gameObject);
            // T?i d? li?u b?ng x?p h?ng ngay khi Manager kh?i t?o.
            LoadLeaderboard();
        }
        else
        {
            // N?u ?ã có instance khác, h?y b? instance hi?n t?i ?? ch? có m?t.
            Destroy(gameObject);
        }
    }

    // T?i d? li?u b?ng x?p h?ng t? PlayerPrefs.
    private void LoadLeaderboard()
    {
        // L?y chu?i JSON t? PlayerPrefs. N?u ch?a có, tr? v? chu?i JSON r?ng c?a m?t ??i t??ng Leaderboard m?i.
        string json = PlayerPrefs.GetString(LeaderboardSaveKey, "{}");
        // Chuy?n ??i chu?i JSON thành ??i t??ng Leaderboard.
        leaderboardData = JsonUtility.FromJson<Leaderboard>(json);

        // N?u vi?c chuy?n ??i th?t b?i ho?c danh sách playerList là null, t?o m?t Leaderboard m?i.
        if (leaderboardData == null || leaderboardData.playerList == null)
        {
            leaderboardData = new Leaderboard();
        }
    }

    // L?u d? li?u b?ng x?p h?ng vào PlayerPrefs.
    private void SaveLeaderboard()
    {
        // S?p x?p danh sách ng??i ch?i theo ?i?m s? gi?m d?n (?i?m cao nh?t lên ??u).
        leaderboardData.playerList = leaderboardData.playerList.OrderByDescending(p => p.score).ToList();

        // Chuy?n ??i ??i t??ng Leaderboard thành chu?i JSON.
        string json = JsonUtility.ToJson(leaderboardData);
        // L?u chu?i JSON vào PlayerPrefs v?i khóa ?ã ??nh ngh?a.
        PlayerPrefs.SetString(LeaderboardSaveKey, json);
        // ??m b?o d? li?u ???c l?u ngay l?p t?c (quan tr?ng cho PlayerPrefs).
        PlayerPrefs.Save();
    }

    // Thêm m?t l??t ch?i m?i vào b?ng x?p h?ng.
    public void AddEntry(PlayerData newEntry)
    {
        leaderboardData.playerList.Add(newEntry); // Thêm l??t ch?i m?i vào danh sách.
        SaveLeaderboard(); // L?u l?i b?ng x?p h?ng sau khi thêm.
    }

    // L?y danh sách các l??t ch?i trong b?ng x?p h?ng (?ã ???c s?p x?p).
    public List<PlayerData> GetLeaderboardEntries()
    {
        // Tr? v? m?t b?n sao c?a danh sách, ??m b?o không thay ??i tr?c ti?p d? li?u g?c.
        return leaderboardData.playerList.OrderByDescending(p => p.score).ToList();
    }

    // (Hàm tùy ch?n) Xóa toàn b? d? li?u b?ng x?p h?ng (h?u ích cho vi?c ki?m th?).
    public void ClearLeaderboard()
    {
        PlayerPrefs.DeleteKey(LeaderboardSaveKey); // Xóa d? li?u t? PlayerPrefs.
        leaderboardData = new Leaderboard(); // Kh?i t?o l?i danh sách r?ng.
    }
}