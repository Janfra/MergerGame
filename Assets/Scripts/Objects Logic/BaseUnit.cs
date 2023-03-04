using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    [SerializeField]
    private SO_GenerateCurrency incomeGeneration;

    #region Unity Functions

    private void OnEnable()
    {
        TurnManager.OnPlayerTurn += GenerateIncome;
    }

    private void OnDisable()
    {
        TurnManager.OnPlayerTurn -= GenerateIncome;
    }

    private void OnDestroy()
    {
        TurnManager.OnPlayerTurn -= GenerateIncome;
    }

    #endregion

    private void GenerateIncome()
    {
        incomeGeneration.GenerateIncome();
    }
}
