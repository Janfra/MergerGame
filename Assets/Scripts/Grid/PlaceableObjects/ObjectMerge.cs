using System;
using UnityEngine;

public class ObjectMerge : PlaceableObject
{
    #region Events

    public static event Action<MergeInformation> OnMergeCommand;
    public static event Action OnFailedMerged;

    #endregion

    #region Variables 

    [Header("Config")]
    [SerializeField]
    private ObjectType type;
    public ObjectType Type => type;
    [SerializeField]
    PlaceableObject mergeResult;

    #endregion
    
    /// <summary>
    /// Checks if object is a merge object, and compatibility to merge.
    /// </summary>
    /// <param name="_objectToMerge">Object to check.</param>
    /// <param name="_placementTile">Tile to place merging result.</param>
    protected void TryToMerge(PlaceableObject _objectToMerge, GridTile _placementTile)
    {
        if(_objectToMerge != this)
        {
            ObjectMerge mergeTo;
            if (mergeTo = TryToConvertTo<ObjectMerge>(_objectToMerge))
            {
                mergeTo.CheckCompatibility(this, _placementTile);
            }
        }
        else
        {
            Debug.Log("Tried to merge to itself");
        }
    }

    /// <summary>
    /// Attempts to merge object with this object. If possible, start merged event.
    /// </summary>
    /// <param name="_mergedObject"></param>
    /// <param name="_placementTile"></param>
    private void CheckCompatibility(ObjectMerge _mergedObject, GridTile _placementTile)
    {
        if (IsValid(_mergedObject.type))
        {
            OnMergeCommand?.Invoke(new MergeInformation(this, _mergedObject, _placementTile));   
        }
        else
        {
            Debug.Log("Merge is not valid");
            OnFailedMerged?.Invoke();
        }
    }

    /// <summary>
    /// Returns if the object type given is valid with this object.
    /// </summary>
    /// <param name="_type">Type to compare.</param>
    /// <returns>Is the merge valid.</returns>
    private bool IsValid(ObjectType _type)
    {
        bool canBeMerged = _type == type;
        return canBeMerged;
    }

    /// <summary>
    /// Spawns merge result and handles the disabling of this object.
    /// </summary>
    /// <returns>Returns the spawned object.</returns>
    public PlaceableObject MergeObjects()
    {
        PlaceableObject resultObject = Instantiate(mergeResult, transform.position, transform.rotation);
        TileChangedTo(null);
        gameObject.SetActive(false);
        return resultObject;
    }

    /// <summary>
    /// Merge types available.
    /// </summary>
    public enum ObjectType
    {
        StartBuilding,
        BasicUnit,
    }

    /// <summary>
    /// Information given on merge event, includes all merging revelant variables.
    /// </summary>
    public struct MergeInformation
    {
        // May add the resulting merge unit for UI purposes
        public readonly ObjectMerge merger;
        public readonly PlaceableObject merged;
        public readonly GridTile resultTile;

        public MergeInformation(ObjectMerge _merger, PlaceableObject _merged, GridTile _resultTile)
        {
            merger = _merger;
            merged = _merged;
            resultTile = _resultTile;
        }
    }
}
