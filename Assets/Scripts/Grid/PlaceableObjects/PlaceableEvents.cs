using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlaceableObject))]
public class PlaceableEvents : MonoBehaviour
{
    #region Events

    /// <summary>
    /// Invoked when object is placed on a tile marked by an event.
    /// </summary>
    public event Action<GridTile, PlaceableObject> OnPlacementEvent;

    /// <summary>
    /// Invoked when this object gets selected.
    /// </summary>
    public event Action OnSelected;

    /// <summary>
    /// Invoked when this object gets deselected.
    /// </summary>
    public event Action OnDeselect;

    #endregion

    private GridTile objectTile; 
    public GridTile ObjectTile => objectTile; 
    private List<GridTile> eventTile = new();
    private static bool isEnabled;

    #region Unity Functions

    private void Awake()
    {
        TurnManager.OnTurnUpdated += UpdateEnable;

        if(TryGetComponent(out PlayerPlacement playerPlacement))
        {
            playerPlacement.OnCommandCreated += ClearEventTiles;
        }
    }

    private void OnDisable()
    {
        TurnManager.OnTurnUpdated -= UpdateEnable;
    }

    private void OnDestroy()
    {
        TurnManager.OnTurnUpdated -= UpdateEnable;
    }

    #endregion

    private void UpdateEnable(int _turnCount, TurnState _turnState)
    {
        isEnabled = _turnState == TurnState.PlayerTurn;
        ClearEventTiles();
    }

    #region Placeable Events

    public void CallPlacementEvent(GridTile _tile, PlaceableObject _placedObject)
    {
        if (isEnabled)
        {
            OnPlacementEvent?.Invoke(_tile, _placedObject);
            ClearEventTiles();
        }
    }

    public void OnFirstSelected()
    {
        CallSelectEvent();
        PlaceableObject.OnSelectedObjectChange += BindDeselect;
    }

    public void OnReselected()
    {
        CallSelectEvent();
    }

    private void CallSelectEvent()
    {
        if (isEnabled)
        {
            OnSelected?.Invoke();
        }
    }

    /// <summary>
    /// Binds deselecting to event, once it gets called, it removes itself from the event. NOTE: Added since Lambdas does not let me remove action from event.
    /// </summary>
    /// <param name="placeableObject"></param>
    private void BindDeselect(PlaceableObject placeableObject)
    {
        Deselect();
        PlaceableObject.OnSelectedObjectChange -= BindDeselect;
    }

    private void Deselect()
    {
        OnDeselect?.Invoke();
        ClearEventTiles();
        Debug.Log($"Deselected {gameObject.name}");
    }

    #endregion

    #region Tile Events

    public void ClearEventTiles()
    {
        foreach (GridTile tile in eventTile)
        {
            tile.TileEvents.ClearOnClick();
            tile.UnHighlight();
        }
        eventTile.Clear();
    }

    /// <summary>
    /// Adds tile to event tiles
    /// </summary>
    /// <param name="_tile"></param>
    public void AddTileToEvent(GridTile _tile)
    {
        if (!eventTile.Contains(_tile))
        {
            eventTile.Add(_tile);
            _tile.Highlight();
        }
    }

    /// <summary>
    /// Add tile to event tiles and sets an on click event.
    /// </summary>
    /// <param name="_tile"></param>
    /// <param name="_onClickableEvent"></param>
    public void AddTileToEvent(GridTile _tile, IOnTileClickableEvent _onClickableEvent)
    {
        if (!eventTile.Contains(_tile))
        {
            eventTile.Add(_tile);
            SetOnClickEvent(_tile, _onClickableEvent);
            _tile.Highlight();
        }
    }

    private void SetOnClickEvent(GridTile _tile, IOnTileClickableEvent _onClickableEvent)
    {
        _tile.TileEvents.SetOnClickedEvent(_onClickableEvent);
    }

    public bool IsTilePartOfEvent(GridTile _tile)
    {
        return eventTile.Contains(_tile);
    }

    #endregion

    public void SetObjectTile(GridTile _tile)
    {
        objectTile = _tile;
    }
}
