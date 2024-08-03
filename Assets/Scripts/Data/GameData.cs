using System;
using System.Collections.Generic;

[Serializable]
public class GameData
{
    public List<PlayerData> players = new List<PlayerData>();
    public int highScore;
}

[Serializable]
public class PlayerData
{
    public string playerName;
    public int score;
}