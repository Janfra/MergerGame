using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private static TurnManager Instance;
    public static event Action<int, TurnState> OnTurnUpdated;
    public static event Action<bool> OnActionsCompleted;

    readonly Dictionary<GameObject, ICommand> turnActions = new();
    readonly Dictionary<GameObject, IChainedCommand> chainedActions = new();
    private Queue<ICommand> actionQueue = new();

    [SerializeField]
    private TurnState currentTurn = TurnState.PlayerTurn;
    private int turnCount = 0;
    private bool areActionsCompleted = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            OnTurnUpdated += (context1, context2) => ExecuteActions();
            ICommand.IsCompleted += StartClearingQueue;

            ObjectMerge.OnMergeCommand += CreateMergeCommand;
            PlaceableObject.OnMoveCommand += CreateMoveCommand;
        }
        else
        {
            Destroy(this);
        }
    }

    #region Commands

    /// <summary>
    /// Creates a movement command to the given object and tile.
    /// </summary>
    /// <param name="movingObject">Object to move.</param>
    /// <param name="placementTile">Tile to place object.</param>
    private void CreateMoveCommand(PlaceableObject movingObject, GridTile placementTile)
    {
        if (areActionsCompleted)
        {
            AddCommand(movingObject.gameObject, new MoveCommand(movingObject, placementTile));
        }
    }

    /// <summary>
    /// Creates a merge command with the merge information given.
    /// </summary>
    /// <param name="_mergeInformation">Variables to create the command.</param>
    private void CreateMergeCommand(ObjectMerge.MergeInformation _mergeInformation)
    {
        if (areActionsCompleted)
        {
            GridTile placementTile = _mergeInformation.resultTile;
            ObjectMerge merger = _mergeInformation.merger;
            PlaceableObject merged = _mergeInformation.merged;

            MergeCommand mergeCommand = new(merger, merged, placementTile);
            AddCommand(merger.gameObject, mergeCommand);
            AddChainedCommand(merged.gameObject, mergeCommand);
        }
    }

    /// <summary>
    /// Adds a command to the list of commands to be done on turn changed.
    /// </summary>
    /// <param name="_commandOwner">Object creating the command</param>
    /// <param name="_command">Command to be done</param>
    public void AddCommand(GameObject _commandOwner, ICommand _command)
    {
        IsObjectChained(_commandOwner);
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

    /// <summary>
    /// Removes and undo the command associated to the given gameobject.
    /// </summary>
    /// <param name="_commandOwner">Gameobject owner of the command to be cancelled.</param>
    public void RemoveCommand(GameObject _commandOwner)
    {
        if (turnActions.ContainsKey(_commandOwner))
        {
            Debug.Log($"Removed action from {_commandOwner.name}");
            turnActions[_commandOwner].Undo();
            turnActions.Remove(_commandOwner);
        }
    }

    /// <summary>
    /// Checks if the object is chained to another action, if true, remove it.
    /// </summary>
    /// <param name="_chainedObject">Object to check</param>
    private void IsObjectChained(GameObject _chainedObject)
    {
        if (chainedActions.ContainsKey(_chainedObject))
        {
            RemoveCommand(chainedActions[_chainedObject].GetOwner());
            chainedActions.Remove(_chainedObject);
        }
    }

    /// <summary>
    /// Checks if object is already chained, then adds a chained action.
    /// </summary>
    /// <param name="_chainedObject"></param>
    /// <param name="_chainedCommand"></param>
    private void AddChainedCommand(GameObject _chainedObject, IChainedCommand _chainedCommand)
    {
        IsObjectChained(_chainedObject);
        chainedActions.Add(_chainedObject, _chainedCommand);
    }

    #endregion

    /// <summary>
    /// Stores actions to be done this turn and start clearing them.
    /// </summary>
    /// <returns></returns>
    private void ExecuteActions()
    {
        SetActionsCompleted(false);
        if(currentTurn == TurnState.PlayerTurn)
        {
            // Execute all commands stored at the start of a new turn
            foreach (var command in turnActions)
            {
                actionQueue.Enqueue(command.Value);
            }

            ClearActions();
            StartClearingQueue();
        }
        else
        {
            SetActionsCompleted(true);
        }
    }

    /// <summary>
    /// Starts executing queue of commands, once clear update to actions completed.
    /// </summary>
    private void StartClearingQueue()
    {
        if(actionQueue.Count > 0)
        {
            ICommand currentCommand = actionQueue.Dequeue();
            currentCommand.Execute();
        }
        else
        {
            SetActionsCompleted(true);
        }
    }

    /// <summary>
    /// Sets if the actions queued are completed and starts event.
    /// </summary>
    /// <param name="_isCompleted">Are actions finished</param>
    private void SetActionsCompleted(bool _isCompleted)
    {
        areActionsCompleted = _isCompleted;
        OnActionsCompleted?.Invoke(areActionsCompleted);
    }

    /// <summary>
    /// Resets actions for next turn actions
    /// </summary>
    private void ClearActions()
    {
        chainedActions.Clear();
        turnActions.Clear();
    }

    /// <summary>
    /// Starts a new turn
    /// </summary>
    public void NextTurn()
    {
        if (areActionsCompleted)
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
