using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementEvents : MonoBehaviour
{
    public event Action<GridTile, PlaceableObject> OnPlacementEvent;
    public event Action OnSelected;

    private GridTile objectTile; 
    public GridTile ObjectTile => objectTile; 
    private List<GridTile> eventTile = new();

    public void CallPlacementEvent(GridTile _tile, PlaceableObject _placedObject)
    {
        OnPlacementEvent?.Invoke(_tile, _placedObject);
    }

    public void CallOnSelectedEvent()
    {
        OnSelected?.Invoke();
    }

    public void ClearEventTiles()
    {
        foreach (GridTile tile in eventTile)
        {
            tile.UnHighlight();
        }
        eventTile.Clear();
    }

    public void AddTileToEvent(GridTile _tile)
    {
        if (!eventTile.Contains(_tile))
        {
            eventTile.Add(_tile);
            _tile.Highlight();
        }
    }

    public bool IsTilePartOfEvent(GridTile _tile)
    {
        return eventTile.Contains(_tile);
    }

    public void SetObjectTile(GridTile _tile)
    {
        objectTile = _tile;
    }
}
