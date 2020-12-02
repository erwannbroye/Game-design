using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TimerController : MonoBehaviour
{
    public static TimerController instance;

    public Text timeCounter;

    private TimeSpan timePlaying;
    private bool timerGoing;
    public UnityEvent onEnd = new UnityEvent();

    public float timeLeft = 100f;
    private float elapsedTime;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (HardMode.hardModeActivated)
            timeCounter.text = "Time: 00:00.00";
        timerGoing = false;
        if (HardMode.hardModeActivated)
            BeginTimer();
    }

    public void BeginTimer()
    {
        timerGoing = true;
        elapsedTime = timeLeft;

        StartCoroutine(UpdateTimer());
    }

    public void EndTimer()
    {
        timerGoing = false;
    }

    private IEnumerator UpdateTimer()
    {
        while (timerGoing)
        {
            if (elapsedTime <= 0) {
                onEnd.Invoke();
            }
            elapsedTime -= Time.deltaTime;
            timePlaying = TimeSpan.FromSeconds(elapsedTime);
            string timePlayingStr = timePlaying.ToString("mm':'ss'.'ff");
            timeCounter.text = timePlayingStr;

            yield return null;
        }
    }
}
