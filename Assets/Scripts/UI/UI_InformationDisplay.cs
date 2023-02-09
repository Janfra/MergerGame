using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_InformationDisplay : MonoBehaviour
{
    [Header("Turn References")]
    [SerializeField]
    private TextMeshProUGUI turnText;
    [SerializeField]
    private TextMeshProUGUI turnCountText;

    [Header("Object References")]
    [SerializeField]
    private GameObject container;
    [SerializeField]
    private TextMeshProUGUI objectName;

    private void Awake()
    {
        TurnManager.OnTurnUpdated += (turnCount, turnState) => UpdateTurnText(turnCount, turnState);
        GameManager.OnSelectedObjectChange += UpdateObjectName;

    }

    private void UpdateObjectName(PlaceableObject _selectedObject)
    {
        objectName.text = _selectedObject.name;
    }

    private void UpdateTurnText(int _turnCount, TurnState _turnState)
    {
        if (turnText == null || turnCountText == null)
            return;

        SetFactionTurn(_turnState);
        SetTurnCount(_turnCount);
    }

    private void SetTurnCount(int _turnCount)
    {
        turnCountText.text = "Turn #" + _turnCount; 
    }

    private void SetFactionTurn(TurnState _turnState)
    {
        switch (_turnState)
        {
            case TurnState.PlayerTurn:
                turnText.text = "Player Turn";

                break;
            case TurnState.EnemyTurn:
                turnText.text = "Enemy Turn";

                break;
            default:
                Debug.LogError("Turn state not set in UI Manager");

                break;
        }
    }
}
