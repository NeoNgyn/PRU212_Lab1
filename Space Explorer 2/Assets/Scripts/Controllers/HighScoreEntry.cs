using System;
using System.Collections.Generic;

[Serializable]
public class HighScoreEntry
{
    public string name;
    public int score;
    public float timePlayed; // Th?i gian ch?i tính b?ng giây
}

[Serializable]
public class HighScores
{
    public List<HighScoreEntry> scores = new List<HighScoreEntry>();
}