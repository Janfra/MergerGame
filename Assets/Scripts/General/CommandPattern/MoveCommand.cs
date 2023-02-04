using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCommand : ICommand
{
    private readonly GridTile movePosition;
    private readonly PlayerPlacement movingObject;

    public MoveCommand(PlayerPlacement _movingObject, GridTile _movePosition)
    {
        movingObject = _movingObject;
        movePosition = _movePosition;

        movingObject.OnMovementFinished += MovementCompleted;
        MarkTileAsOccupied();
    }

    public void Execute()
    {
        Debug.Log("Move command executed");
        ICommand.IsCompleted = false;
        movingObject.PlaceOnTile(movePosition);
    }

    public void Undo()
    {
        MarkTileAsOccupied(false);
        movingObject.OnMovementFinished -= MovementCompleted;
    }

    private void MarkTileAsOccupied(bool _isOccupied = true)
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
