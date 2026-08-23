using UnityEngine;

// Tracks which gameplay scene the player was last in, so Game Over's
// Retry can reload the actual current level instead of always restarting
// from the beginning.
public static class GameProgress
{
    private const string CurrentLevelKey = "CurrentLevel";
    private const string DefaultLevel = "TutorialLevel";

    public static void SetCurrentLevel(string sceneName)
    {
        PlayerPrefs.SetString(CurrentLevelKey, sceneName);
    }

    public static string GetCurrentLevel()
    {
        return PlayerPrefs.GetString(CurrentLevelKey, DefaultLevel);
    }
}
