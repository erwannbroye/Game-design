using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    // Start is called before the first frame update
    public LevelManager levelManager;
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
        if (!levelManager) {
            Debug.Log("level manager doesn't exist");
            return;
        }
        if (other.tag == "Player"){
            Debug.Log("Level Saved");
            levelManager.currentLevel += 1;
            PlayerPrefs.SetInt("levelAt", levelManager.currentLevel);
            SaveSystem.SaveLevel(levelManager);
            levelManager.currentLevel -= 1;
            levelManager.LoadSceneLevel();
        }
    }
}
