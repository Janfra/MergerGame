using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Income Stat", menuName = "Scriptable Objects/Income Generation")]
public class SO_GenerateCurrency : ScriptableObject
{
    [SerializeField]
    protected int currencyPerGeneration = 1;

    public virtual void GenerateIncome()
    {
        PlayerCurrency.Instance.AddCurrency(currencyPerGeneration);
    }
}
