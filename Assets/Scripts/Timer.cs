using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Timer
{
    #region Variables

    [SerializeField] private float totalTime = 1;
    public float TotalTime { get { return totalTime; } }
    public float CurrentTime { get; private set; }
    public bool IsTimerDone { get; private set; }

    #endregion

    /// <summary>
    /// Constructor 
    /// </summary>
    public Timer()
    {
        totalTime = 1f;
        CurrentTime = 0f;
        IsTimerDone = true;
    }

    /// <summary>
    /// Constructor 
    /// </summary>
    /// <param name="timerTotalTime"></param>
    public Timer(float timerTotalTime)
    {
        SetTotalTime(timerTotalTime);
        SetCurrentTime(0f);
        IsTimerDone = true;
    }

    /// <summary>
    /// Sets timer target time and cancels old timer.
    /// </summary>
    /// <param name="timerTotalTime">New timer target time</param>
    public void SetTimer(float timerTotalTime)
    {
        CancelTimer();
        SetTotalTime(timerTotalTime);
        SetCurrentTime(0f);
    }

    /// <summary>
    /// Sets timer target times and decides whether to cancel old timer
    /// </summary>
    /// <param name="timerTotalTime">New timer target time</param>
    /// <param name="cancelCurrentTimer">Cancel timer</param>
    public void SetTimer(float timerTotalTime, bool cancelCurrentTimer)
    {
        if(cancelCurrentTimer)
        {
            CancelTimer();
            SetCurrentTime(0f);
        }
        else if(timerTotalTime < CurrentTime)
        {
            CancelTimer();
            SetCurrentTime(timerTotalTime);
            Debug.LogWarning("The new timer total time is lower than the current time, timer has been canceled!");
        }
        SetTotalTime(timerTotalTime);
    }

    /// <summary>
    /// Sets time target time
    /// </summary>
    /// <param name="newTimerTime"></param>
    private void SetTotalTime(float newTimerTime)
    {
        if(newTimerTime > 0)
        {
            totalTime = newTimerTime;
        }
        else
        {
            Debug.LogWarning("Timer cannot be set under 0! Defaulted to 1");
            totalTime = 1;
        }
    }

    /// <summary>
    /// Sets timer current time
    /// </summary>
    /// <param name="newCurrentTime"></param>
    private void SetCurrentTime(float newCurrentTime)
    {
        if(newCurrentTime <= totalTime)
        {
            CurrentTime = newCurrentTime;
        }
        else
        {
            CurrentTime = totalTime;
            // Debug.LogWarning($"Tried to set current time over total time! Value: {newCurrentTime}");
        }
    }

    /// <summary>
    /// Starts a coroutine for the timer.
    /// </summary>
    /// <typeparam name="T">Generic Monobehaviour to support running the coroutine</typeparam>
    /// <param name="caller">Timer owner</param>
    public void StartTimer<T>(T caller) where T : MonoBehaviour 
    {
        if(IsTimerDone == true)
        {
            IsTimerDone = false;
            caller.StartCoroutine(RunTimer());
        }
        else
        {
            Debug.Log("Timer is not done!");
        }
    }

    /// <summary>
    /// Stops timer
    /// </summary>
    public void CancelTimer()
    {
        IsTimerDone = true;
    }

    /// <summary>
    /// Updates current time until target is reached
    /// </summary>
    /// <returns></returns>
    private IEnumerator RunTimer()
    {
        while (CurrentTime < totalTime && !IsTimerDone)
        {
            SetCurrentTime(CurrentTime + Time.deltaTime);
            yield return null;
        }
        IsTimerDone = true;
        SetCurrentTime(0f);
        // Debug.Log($"Timer has finished! Can be started again. Time runned: {totalTime}");
        yield return null;
    }
}
