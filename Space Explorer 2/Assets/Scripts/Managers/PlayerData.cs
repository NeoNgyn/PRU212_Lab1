// File: Assets/Scripts/Data/PlayerData.cs
using System; // C?n thi?t cho [Serializable]
using System.Collections.Generic; // C?n thi?t cho List trong l?p Leaderboard

// ?ánh d?u [Serializable] ?? Unity có th? chuy?n ??i ??i t??ng này thành ??nh d?ng JSON.
// ?i?u này quan tr?ng ?? l?u và t?i d? li?u PlayerData.
[Serializable]
public class PlayerData
{
    public string playerName;   // Tên ng??i ch?i
    public int score;           // ?i?m s? ??t ???c
    public float timePlayed;    // Th?i gian ch?i trong l??t ?ó
}

// L?p bao b?c danh sách PlayerData.
// JsonUtility c?n m?t l?p bao b?c khi b?n mu?n l?u/t?i m?t List các ??i t??ng.
[Serializable]
public class Leaderboard
{
    public List<PlayerData> playerList = new List<PlayerData>(); // Danh sách các l??t ch?i ?ã l?u
}