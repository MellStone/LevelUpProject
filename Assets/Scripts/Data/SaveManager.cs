using System.IO;
using UnityEngine;

public static class SaveManager
{
    private static readonly string SaveFilePath = Path.Combine(Application.persistentDataPath, "gameData.json");

    public static void SaveGame(GameData gameData)
    {
        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(SaveFilePath, json);
    }

    public static GameData LoadGame()
    {
        if (File.Exists(SaveFilePath))
        {
            string json = File.ReadAllText(SaveFilePath);
            return JsonUtility.FromJson<GameData>(json);
        }
        return new GameData();
    }

    public static void SaveHighScore(int highScore)
    {
        PlayerPrefs.SetInt("HighScore", highScore);
    }

    public static int LoadHighScore()
    {
        return PlayerPrefs.GetInt("HighScore", 0);
    }
}