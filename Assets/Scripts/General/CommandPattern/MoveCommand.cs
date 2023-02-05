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

        movingObject.OnMovementFinished += MovementCompleted;

        if (isMarkedAsOccupied)
        {
            MarkTileAsOccupied(true);
        }
    }

    public void Execute()
    {
        Debug.Log($"Move command executed by {movingObject.name}");
        ICommand.IsCompleted = false;
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

    private void MarkTileAsOccupied(bool _isOccupied)
    {
        // Assuming that the tile was not occupied and can be marked and unmarked
        if (_isOccupied)
        {
            Debug.Log($"{movePosition.name} was set as will be occupied by command");
            movePosition.SetOccupyingObject(movingObject, false);
            movePosition.HighlightDelayed();
        }
        else
        {
            Debug.Log($"{movePosition.name} was set as not occupied by command");
            movePosition.SetOccupyingObject(null, false);
            movePosition.UnHighlight();
        }
    }
    
    private void MovementCompleted()
    {
        ICommand.IsCompleted = true;
    }
}
