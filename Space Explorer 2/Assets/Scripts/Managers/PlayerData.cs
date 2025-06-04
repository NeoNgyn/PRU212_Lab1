// File: Assets/Scripts/Data/PlayerData.cs
using System; // C?n thi?t cho [Serializable]
using System.Collections.Generic; // C?n thi?t cho List trong l?p Leaderboard

// ?�nh d?u [Serializable] ?? Unity c� th? chuy?n ??i ??i t??ng n�y th�nh ??nh d?ng JSON.
// ?i?u n�y quan tr?ng ?? l?u v� t?i d? li?u PlayerData.
[Serializable]
public class PlayerData
{
    public string playerName;   // T�n ng??i ch?i
    public int score;           // ?i?m s? ??t ???c
    public float timePlayed;    // Th?i gian ch?i trong l??t ?�
}

// L?p bao b?c danh s�ch PlayerData.
// JsonUtility c?n m?t l?p bao b?c khi b?n mu?n l?u/t?i m?t List c�c ??i t??ng.
[Serializable]
public class Leaderboard
{
    public List<PlayerData> playerList = new List<PlayerData>(); // Danh s�ch c�c l??t ch?i ?� l?u
}