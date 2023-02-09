using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Turn References")]
    [SerializeField]
    private TextMeshProUGUI turnText;
    [SerializeField]
    private TextMeshProUGUI turnCountText;

    [Header("Object Info References")]
    [SerializeField]
    private GameObject objectInfoContainer;

    private void Awake()
    {
        TurnManager.OnTurnUpdated += (turnCount, turnState) => UpdateTurnText(turnCount, turnState);
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
