using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Timer
{
    #region Variables

    [Header("Config")]
    [SerializeField] private float targetTime = 1;
    public float TargetTime { get { return targetTime; } }
    public float CurrentTime { get; private set; }
    public bool IsTimerDone { get; private set; }
    public bool IsTimerPaused { get; private set; }

    private Action OnTimerDone;

    #endregion

    /// <summary>
    /// Constructor 
    /// </summary>
    public Timer()
    {
        targetTime = 1f;
        CurrentTime = 0f;
        IsTimerDone = true;
    }

    /// <summary>
    /// Constructor 
    /// </summary>
    /// <param name="_timerTargetTime">Sets the timer duration</param>
    public Timer(float _timerTargetTime)
    {
        SetTargetTime(_timerTargetTime);
        SetCurrentTime(0f);
        IsTimerDone = true;
    }

    /// <summary>
    /// Sets timer target time and cancels old timer.
    /// </summary>
    /// <param name="_timerTargetTime">New timer target time</param>
    public void SetTimer(float _timerTargetTime)
    {
        CancelTimer();
        SetTargetTime(_timerTargetTime);
        SetCurrentTime(0f);
    }

    /// <summary>
    /// Sets timer target times and decides whether to cancel old timer
    /// </summary>
    /// <param name="_timerTargetTime">New timer target time / duration</param>
    /// <param name="_cancelCurrentTimer">Cancel the old timer</param>
    public void SetTimer(float _timerTargetTime, bool _cancelCurrentTimer = true)
    {
        if (_cancelCurrentTimer)
        {
            CancelTimer();
            SetCurrentTime(0f);
        }
        else if (_timerTargetTime < CurrentTime)
        {
            CancelTimer();
            SetCurrentTime(_timerTargetTime);
            Debug.LogWarning("The new timer total time is lower than the current time, timer has been canceled!");
        }
        SetTargetTime(_timerTargetTime);
    }

    /// <summary>
    /// Sets timer target time and cancels old timer.
    /// </summary>
    /// <param name="_timerTargetTime">New timer target time</param>
    public void SetTimer(float _timerTargetTime, Action _onTimerFinished)
    {
        CancelTimer();
        SetTargetTime(_timerTargetTime);
        SetCurrentTime(0f);
        OnTimerDone = _onTimerFinished;
    }

    /// <summary>
    /// Sets time target time
    /// </summary>
    /// <param name="_TargetTime"></param>
    private void SetTargetTime(float _TargetTime)
    {
        if (_TargetTime >= 0)
        {
            targetTime = _TargetTime;
        }
        else
        {
            Debug.LogWarning("Timer cannot be set under 0! Defaulted to 1");
            targetTime = 1;
        }
    }

    /// <summary>
    /// Sets timer current time
    /// </summary>
    /// <param name="_currentTime"></param>
    private void SetCurrentTime(float _currentTime)
    {
        if (_currentTime <= targetTime)
        {
            CurrentTime = _currentTime;
        }
        else
        {
            CurrentTime = targetTime;
            // Debug.LogWarning($"Tried to set current time over total time! Value: {newCurrentTime}");
        }
    }

    /// <summary>
    /// Starts a coroutine for the timer.
    /// </summary>
    /// <typeparam name="T">Generic Monobehaviour to support running the coroutine</typeparam>
    /// <param name="_caller">Timer owner</param>
    public void StartTimer<T>(T _caller) where T : MonoBehaviour
    {
        if (IsTimerDone == true)
        {
            IsTimerDone = false;
            IsTimerPaused = false;
            _caller.StartCoroutine(RunTimer());
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
    /// Sets whether to pause the timer or not if active.
    /// </summary>
    /// <param name="_isTimerPaused">Is timer paused or not</param>
    public void PauseTimer(bool _isTimerPaused)
    {
        if (!IsTimerDone)
        {
            IsTimerPaused = _isTimerPaused;
        }
        else
        {
            Debug.LogWarning("No timer currently active");
        }
    }

    /// <summary>
    /// Updates current time until target is reached
    /// </summary>
    /// <returns></returns>
    private IEnumerator RunTimer()
    {
        while (CurrentTime < targetTime && !IsTimerDone)
        {
            if (!IsTimerPaused)
            {
                SetCurrentTime(CurrentTime + Time.deltaTime);
            }
            yield return null;
        }
        IsTimerDone = true;
        OnTimerDone?.Invoke();
        SetCurrentTime(0f);
        // Debug.Log($"Timer has finished! Can be started again. Time runned: {totalTime}");
        yield return null;
    }

    /// <summary>
    /// Normalizes 'alpha' with the total duration. Modified to work with fractions.
    /// </summary>
    /// <param name="_alpha">Value to normalize</param>
    /// <param name="_duration">Max value of the normalize formula</param>
    /// <returns>A value in between 0 and 1, the duration being 1</returns>
    private float NormalizeTime(float _alpha, float _duration)
    {
        // 1 is added to everything to avoid dividing under 0 and getting unexpected values
        int minValue = 1;
        _alpha += minValue;
        _duration += minValue;
        return (_alpha - minValue) / (_duration - minValue);
    }

    /// <summary>
    /// Returns the current time normalized going from 0-1.
    /// </summary>
    /// <returns>Current time normalized to be 0-1</returns>
    public float GetTimeNormalized()
    {
        return NormalizeTime(CurrentTime, TargetTime);
    }

    /// <summary>
    /// Returns the current time normalized going from 1-0.
    /// </summary>
    /// <returns>Current time normalized to be 1-0</returns>
    public float GetReversedTimeNormalized()
    {
        return NormalizeTime(TargetTime - CurrentTime, TargetTime);
    }
}