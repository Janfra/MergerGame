using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCommand : ICommand
{
    GridTile movePosition;
    PlaceableObject movingObject;

    public void Execute()
    {
        
    }

    public void Undo()
    {
        throw new System.NotImplementedException();
    }
}
