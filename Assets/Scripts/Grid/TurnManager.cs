using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    public static event Action<int, TurnState> OnTurnUpdated;

    readonly Dictionary<GameObject, ICommand> turnActions = new();
    readonly List<GameObject> affectedObjects = new();

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

            ObjectMerge.OnMergeCommand += CreateMergeCommand;
            PlaceableObject.OnMoveCommand += CreateMoveCommand;
        }
        else
        {
            Destroy(this);
        }
    }


    /// NOTE: Updating from ICommand to ICommandAction, NOT FINISHED.
    #region Commands

    /// <summary>
    /// Creates a movement command to the given object and tile.
    /// </summary>
    /// <param name="movingObject">Object to move.</param>
    /// <param name="placementTile">Tile to place object.</param>
    private void CreateMoveCommand(PlaceableObject movingObject, GridTile placementTile)
    {
        AddCommand(movingObject.gameObject, new MoveCommand(movingObject, placementTile));
    }

    /// <summary>
    /// Creates a merge command with the merge information given.
    /// </summary>
    /// <param name="_mergeInformation">Variables to create the command.</param>
    private void CreateMergeCommand(ObjectMerge.MergeInformation _mergeInformation)
    {
        GridTile placementTile = _mergeInformation.resultTile;
        ObjectMerge merger = _mergeInformation.merger;
        PlaceableObject merged = _mergeInformation.merged;

        AddCommand(gameObject, new MoveCommand(merged, placementTile));
        AddCommand(merged.gameObject, new MergeCommand(merger, merged.gameObject, placementTile));
    }

    /// <summary>
    /// Adds a command to the list of commands to be done on turn changed.
    /// </summary>
    /// <param name="_commandOwner">Object creating the command</param>
    /// <param name="_command">Command to be done</param>
    public void AddCommand(GameObject _commandOwner, ICommand _command)
    {
        CheckGameobjectAvailability(_commandOwner);
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
            turnActions[_commandOwner].Undo();
            turnActions.Remove(_commandOwner);
            affectedObjects.Remove(_commandOwner);
        }
    }

    /// <summary>
    /// Checks if the given gameobject is not part of a command, otherwise undo old command.
    /// </summary>
    /// <param name="_commandOwner"></param>
    private void CheckGameobjectAvailability(GameObject _commandOwner)
    {
        if (!affectedObjects.Contains(_commandOwner))
        {
            affectedObjects.Add(_commandOwner);
        }
        else
        {
            
        }
    }

    #endregion

    private IEnumerator ExecuteActions()
    {
        isTurnFinished = false;
        ICommand.IsCompleted = true;

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

            affectedObjects.Clear();
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
