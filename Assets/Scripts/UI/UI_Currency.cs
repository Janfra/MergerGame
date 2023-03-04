using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Currency : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI currencyText;
    [SerializeField]
    private Animator currencyAnimation;

    #region Unity Functions

    private void OnEnable()
    {
        PlayerCurrency.OnCurrencyUpdated += UpdateCurrency;
    }

    private void OnDisable()
    {
        PlayerCurrency.OnCurrencyUpdated -= UpdateCurrency;
    }

    private void OnDestroy()
    {
        PlayerCurrency.OnCurrencyUpdated -= UpdateCurrency;
    }

    #endregion

    private void UpdateCurrency(int _newAmount)
    {
        currencyText.text = $"Money: {_newAmount}";   
    }
}
