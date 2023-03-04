using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCurrency : MonoBehaviour
{
    #region Events

    public static Action<int> OnCurrencyUpdated;

    #endregion

    public static PlayerCurrency Instance;

    [SerializeField]
    private int earnedCurrency;
    public int Currency => earnedCurrency;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            ResetCurrency();
        }
        else
        {
            Destroy(this);
        }
    }

    public void ResetCurrency()
    {
        earnedCurrency = 0;
        OnCurrencyUpdated?.Invoke(earnedCurrency);
    }

    public void RemoveCurrency(int _removedAmount)
    {
        earnedCurrency -= _removedAmount;
        OnCurrencyUpdated?.Invoke(earnedCurrency);
    }

    public void AddCurrency(int _earnedAmount)
    {
        earnedCurrency += _earnedAmount;
        OnCurrencyUpdated?.Invoke(earnedCurrency);
    }
}
