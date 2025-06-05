// File: Assets/Scripts/Managers/LeaderboardManager.cs
using UnityEngine;
using System.Collections.Generic; // C?n thi?t cho List
using System.Linq; // C?n thi?t cho OrderByDescending (?? s?p x?p)

public class LeaderboardManager : MonoBehaviour
{
    // Thi?t k? Singleton: ??m b?o ch? c� m?t th? hi?n c?a LeaderboardManager t?n t?i.
    public static LeaderboardManager instance;

    private Leaderboard leaderboardData; // ??i t??ng ch?a to�n b? d? li?u b?ng x?p h?ng
    private const string LeaderboardSaveKey = "LeaderboardData"; // Kh�a d�ng ?? l?u v�o PlayerPrefs

    private void Awake()
    {
        // Ki?m tra xem ?� c� instance n�o c?a LeaderboardManager ch?a.
        if (instance == null)
        {
            instance = this; // G�n instance n�y l�m th? hi?n duy nh?t.
            // Gi? cho GameObject n�y kh�ng b? h?y khi chuy?n ??i gi?a c�c Scene.
            DontDestroyOnLoad(gameObject);
            // T?i d? li?u b?ng x?p h?ng ngay khi Manager kh?i t?o.
            LoadLeaderboard();
        }
        else
        {
            // N?u ?� c� instance kh�c, h?y b? instance hi?n t?i ?? ch? c� m?t.
            Destroy(gameObject);
        }
    }

    // T?i d? li?u b?ng x?p h?ng t? PlayerPrefs.
    private void LoadLeaderboard()
    {
        // L?y chu?i JSON t? PlayerPrefs. N?u ch?a c�, tr? v? chu?i JSON r?ng c?a m?t ??i t??ng Leaderboard m?i.
        string json = PlayerPrefs.GetString(LeaderboardSaveKey, "{}");
        // Chuy?n ??i chu?i JSON th�nh ??i t??ng Leaderboard.
        leaderboardData = JsonUtility.FromJson<Leaderboard>(json);

        // N?u vi?c chuy?n ??i th?t b?i ho?c danh s�ch playerList l� null, t?o m?t Leaderboard m?i.
        if (leaderboardData == null || leaderboardData.playerList == null)
        {
            leaderboardData = new Leaderboard();
        }
    }

    // L?u d? li?u b?ng x?p h?ng v�o PlayerPrefs.
    private void SaveLeaderboard()
    {
        // S?p x?p danh s�ch ng??i ch?i theo ?i?m s? gi?m d?n (?i?m cao nh?t l�n ??u).
        leaderboardData.playerList = leaderboardData.playerList.OrderByDescending(p => p.score).ToList();

        // Chuy?n ??i ??i t??ng Leaderboard th�nh chu?i JSON.
        string json = JsonUtility.ToJson(leaderboardData);
        // L?u chu?i JSON v�o PlayerPrefs v?i kh�a ?� ??nh ngh?a.
        PlayerPrefs.SetString(LeaderboardSaveKey, json);
        // ??m b?o d? li?u ???c l?u ngay l?p t?c (quan tr?ng cho PlayerPrefs).
        PlayerPrefs.Save();
    }

    // Th�m m?t l??t ch?i m?i v�o b?ng x?p h?ng.
    public void AddEntry(PlayerData newEntry)
    {
        leaderboardData.playerList.Add(newEntry); // Th�m l??t ch?i m?i v�o danh s�ch.
        SaveLeaderboard(); // L?u l?i b?ng x?p h?ng sau khi th�m.
    }

    // L?y danh s�ch c�c l??t ch?i trong b?ng x?p h?ng (?� ???c s?p x?p).
    public List<PlayerData> GetLeaderboardEntries()
    {
        // Tr? v? m?t b?n sao c?a danh s�ch, ??m b?o kh�ng thay ??i tr?c ti?p d? li?u g?c.
        return leaderboardData.playerList.OrderByDescending(p => p.score).ToList();
    }

    // (H�m t�y ch?n) X�a to�n b? d? li?u b?ng x?p h?ng (h?u �ch cho vi?c ki?m th?).
    public void ClearLeaderboard()
    {
        PlayerPrefs.DeleteKey(LeaderboardSaveKey); // X�a d? li?u t? PlayerPrefs.
        leaderboardData = new Leaderboard(); // Kh?i t?o l?i danh s�ch r?ng.
    }
}