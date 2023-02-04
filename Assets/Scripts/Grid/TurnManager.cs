using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    public static event Action<int, TurnState> OnTurnUpdated;

    Dictionary<GameObject, ICommand> turnActions = new();
    [SerializeField]
    private TurnState currentTurn = TurnState.PlayerTurn;
    private int turnCount = 0;
    private bool isTurnFinished = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            OnTurnUpdated += (context1, context2) => StartCoroutine(ExecuteActions());
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
            turnActions[_commandOwner].Undo();
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
            turnActions[_commandOwner].Undo();
            turnActions.Remove(_commandOwner);
        }
    }

    private IEnumerator ExecuteActions()
    {
        isTurnFinished = false;
        if(currentTurn == TurnState.PlayerTurn)
        {
            // Execute all commands stored at the start of a new turn
            foreach (var command in turnActions)
            {
                while (!ICommand.IsCompleted)
                {
                    yield return null;
                }
                command.Value.Execute();
                yield return null;
            }

            turnActions.Clear();
            yield return null;
        }

        isTurnFinished = true;
        yield return null;
    }

    public void NextTurn()
    {
        if (isTurnFinished)
        {
            turnCount++;
            TurnState stateUpdate = (TurnState)(turnCount % (int)TurnState.TOTAL_TURN_STATES);
            currentTurn = stateUpdate;
            OnTurnUpdated?.Invoke(turnCount, currentTurn);
        }
    }
}

[System.Serializable]
public enum TurnState
{
    PlayerTurn,
    EnemyTurn,
    TOTAL_TURN_STATES,
}
