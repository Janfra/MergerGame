using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeCommand : IChainedCommand
{
    private readonly ObjectMerge objectMerger;
    private readonly PlaceableObject objectMerged;
    private readonly GridTile tile;
    private ICommand moveToMerge;
    public ICommand ChainedCommand { get => moveToMerge; set => moveToMerge = value; }

    public MergeCommand(ObjectMerge _objectMerger, PlaceableObject _objectMerged, GridTile _tile)
    {
        objectMerger = _objectMerger;
        objectMerged = _objectMerged;
        tile = _tile;
        ChainedCommand = new MoveCommand(_objectMerged, _tile, false);
    }


    public void Execute()
    {
        objectMerged.OnMovementFinished += GenerateMerge;
        moveToMerge.Execute();
    }

    public GameObject GetOwner()
    {
        return objectMerger.gameObject;
    }

    public void Undo()
    {
        moveToMerge.Undo();
    }

    private void GenerateMerge()
    {
        objectMerged.gameObject.SetActive(false);
        PlaceableObject mergeResult = objectMerger.MergeObjects();
        tile.SetOccupyingObject(mergeResult);
    }
}
