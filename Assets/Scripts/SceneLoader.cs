using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public AudioMixer audiomixer;
    public Button[] lvlButtons;
    private LevelManager lvlManager;

    void Start()
    {
        int lvlAt = PlayerPrefs.GetInt("levelAt", 1);

        for (int i = 0; i < lvlButtons.Length; ++i)
        {
            if (i + 1 > lvlAt)
            {
                lvlButtons[i].interactable = false;
            }

        }
    }

    private void Awake() {
        HardMode.hardModeActivated = false;
    }

    public void ActiveHardMode()
    {
        HardMode.hardModeActivated = !HardMode.hardModeActivated;
    }

    public void NewGame()
    {
        PlayerPrefs.SetInt("levelAt", 1);
        lvlManager.LoadSceneLevel(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetVolume(float value)
    {
        audiomixer.SetFloat("Volume", value);
    }
}
