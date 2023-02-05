using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeCommand : ICommandAction
{
    public GameObject[] ObjectsAffected { get => objectsAffected; set => ObjectsAffected = objectsAffected; }
    private GameObject[] objectsAffected;
    private readonly ObjectMerge objectMerger;
    private readonly GameObject objectMerged;
    private readonly GridTile tile;

    public MergeCommand(ObjectMerge _objectMerger, GameObject _objectMerged, GridTile _tile)
    {
        objectMerger = _objectMerger;
        objectMerged = _objectMerged;
        tile = _tile;
    }


    public void Execute()
    {
        objectMerged.SetActive(false);
        PlaceableObject mergeResult = objectMerger.MergeObjects();
        tile.SetOccupyingObject(mergeResult);
    }

    public void Undo()
    {
        
    }
}
