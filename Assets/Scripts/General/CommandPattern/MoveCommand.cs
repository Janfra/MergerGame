using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCommand : ICommand
{
    private readonly GridTile movePosition;
    private readonly PlaceableObject movingObject;
    private readonly bool isMarkedAsOccupied;

    public MoveCommand(PlaceableObject _movingObject, GridTile _movePosition, bool _isMarkedAsOccupied = true)
    {
        movingObject = _movingObject;
        movePosition = _movePosition;
        isMarkedAsOccupied = _isMarkedAsOccupied;

        // Event to know when the object finished moving
        movingObject.OnMovementFinished += MovementCompleted;

        if (isMarkedAsOccupied)
        {
            MarkTileAsOccupied(true);
        }
    }

    public void Execute()
    {
        Debug.Log($"Move command executed by {movingObject.name}");
        movingObject.PlaceOnTile(movePosition);
    }

    public void Undo()
    {
        if (isMarkedAsOccupied)
        {
            MarkTileAsOccupied(false);
        }
        movingObject.OnMovementFinished -= MovementCompleted;
    }

    /// <summary>
    /// Sets whether to mark the tile to place object as taken
    /// </summary>
    /// <param name="_isOccupied"></param>
    private void MarkTileAsOccupied(bool _isOccupied)
    {
        // Assuming that the tile was not occupied and can be marked and unmarked
        if (_isOccupied)
        {
            Debug.Log($"{movePosition.name} was set as will be occupied by command");
            movePosition.SetOccupyingObject(movingObject, false);
        }
        else
        {
            Debug.Log($"{movePosition.name} was set as not occupied by command");
            movePosition.SetOccupyingObject(null, false);
            movePosition.UnHighlight();
        }
    }
    
    /// <summary>
    /// Compete command once movement is finished
    /// </summary>
    private void MovementCompleted()
    {
        ICommand.CommandCompleted();
    }

    public override string ToString()
    {
        return "Moving";
    }
}
