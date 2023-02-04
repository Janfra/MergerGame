using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    public static event Action<int, TurnState> OnTurnUpdated;

    [SerializeField]
    private TurnState currentTurn = TurnState.PlayerTurn;
    private int turnCount = 0;
    Dictionary<GameObject, ICommand> turnActions = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            OnTurnUpdated += (context1, context2) => ExecuteActions();
        }
        else
        {
            Destroy(this);
        }
    }

    public void AddCommand(GameObject _commandOwner, ICommand _command)
    {
        if (turnActions.ContainsKey(_commandOwner))
        {
            Debug.Log($"Overwrote old command from {_commandOwner.name}");
            turnActions[_commandOwner] = _command;
        }
        else
        {
            Debug.Log($"New command from {_commandOwner.name}");
            turnActions.Add(_commandOwner, _command);
        }
    }

    public void RemoveCommand(GameObject _commandOwner)
    {
        if (turnActions.ContainsKey(_commandOwner))
        {
            turnActions.Remove(_commandOwner);
        }
    }

    private void ExecuteActions()
    {
        if(currentTurn == TurnState.PlayerTurn)
        {
            // Execute all commands stored at the start of a new turn
            foreach (var command in turnActions)
            {
                command.Value.Execute();
            }
        }

        turnActions.Clear();
    }

    public static void NextTurn()
    {
        Instance.turnCount++;
        TurnState stateUpdate = (TurnState)(Instance.turnCount % (int)TurnState.TOTAL_TURN_STATES);
        Instance.currentTurn = stateUpdate;
        OnTurnUpdated?.Invoke(Instance.turnCount, Instance.currentTurn);
    }
}

[System.Serializable]
public enum TurnState
{
    PlayerTurn,
    EnemyTurn,
    TOTAL_TURN_STATES,
}
