using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    // Start is called before the first frame update
    public int currentLevel;
    public string level_name_prefix = "level_";

    public void restartLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Physics.gravity = new Vector3(0, -30, 0);
    }

    public void SaveLevel()
    {
        SaveSystem.SaveLevel(this);
    }

    public void LoadCurrentLevel()
    {
        LevelData data = SaveSystem.loadLevel();

        currentLevel = data.level;
    }

    public void LoadSceneLevel()
    {
        LoadCurrentLevel();
        SceneManager.LoadScene(level_name_prefix + currentLevel);
    }

    public void LoadSceneLevel(int level)
    {
        SceneManager.LoadScene(level_name_prefix + level);
    }
}

[System.Serializable]
public class LevelData {
    public int level;

    public LevelData(LevelManager levelManager) {
        level = levelManager.currentLevel;
    }
}