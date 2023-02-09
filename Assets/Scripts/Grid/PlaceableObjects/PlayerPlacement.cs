using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlacement : ObjectMerge
{
    [SerializeField] 
    private MouseDragHandler dragHandler;
    [SerializeField]
    private LayerMask TileMask = 1 << 6;

    /// <summary>
    /// Sets if the player can start logic.
    /// </summary>
    private bool isEnabled = true;

    private void Awake()
    {
        dragHandler.OnAwake(GameGrid.TILESIZE);

        // Temporary default positioning
        SetDefaultPosition(Vector3.zero + Vector3.up);

        TurnManager.OnActionsCompleted += SetEnable;
    }

    private void OnDestroy()
    {
        TurnManager.OnActionsCompleted -= SetEnable;
    }

    private void OnMouseDown()
    {
        GameManager.SetSelectedObject(this);
    }

    private void OnMouseDrag()
    {
        OnDragObject();
    }

    private void OnMouseUp()
    {
        DropToTile();
    }

    #region Tile Selection

    /// <summary>
    /// Dragging logic for object.
    /// </summary>
    protected virtual void OnDragObject()
    {
        if(!isEnabled)
            return;

        if (!dragHandler.OnDragObjectOnTile(gameObject.transform))
        {
            dragHandler.OnDragObject(gameObject.transform);
            HighlightDropTile();
        }
    }

    /// <summary>
    /// Attempts to return the tile under this object.
    /// </summary>
    /// <returns>Returns tile if found, otherwise null.</returns>
    private GridTile GetTileUnderObject()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(-transform.up), out RaycastHit hit, Mathf.Infinity, TileMask))
        {
            if (hit.collider.TryGetComponent(out GridTile selectedTile))
            {
                return selectedTile;
            }
        }
        return null;
    }

    /// <summary>
    /// Highlight tile the object will be dropped on.
    /// </summary>
    private void HighlightDropTile()
    {
        GridTile selectedTile = GetTileUnderObject();
        if (selectedTile)
        {
            selectedTile.StartHighlightTimer();
        }
    }

    /// <summary>
    /// Attempts to drop the object to a tile, if not possible, set back to initial position.
    /// </summary>
    private void DropToTile()
    {
        if (!isEnabled)
            return;

        GridTile selectedTile = GetTileUnderObject();
        if (selectedTile)
        {
            foreach(GridTile tile in selectedTile.GetTileNeighbours())
            {
                tile.Highlight();
            }
            OnTileFound(selectedTile);
        }
        else
        {
            OnTileNotFound();
        }
    }

    /// <summary>
    /// Creates a command depending on the tile information.
    /// </summary>
    /// <param name="_tileFound">Tile to check information.</param>
    protected virtual void OnTileFound(GridTile _tileFound)
    {
        GoBackToDefaultPosition();
        if (_tileFound.IsOccupied)
        {
            if (_tileFound.IsPlaced) // && same faction
            {
                TryToMerge(_tileFound.OccupyingObject, _tileFound);
            }
        }
        else
        {
            CreateMoveCommand(_tileFound);
        }
    }

    /// <summary>
    /// Error handling for tile not found.
    /// </summary>
    private void OnTileNotFound()
    {
        GoBackToDefaultPosition();
        Debug.Log("No tile detected!");
    }

    #endregion

    /// <summary>
    /// Sets if this object is currently enabled for the player to interact with.
    /// </summary>
    /// <param name="_isEnabled">Is object usable by player.</param>
    public void SetEnable(bool _isEnabled)
    {
        isEnabled = _isEnabled;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Ray drawRay = new(transform.position, transform.TransformDirection(-transform.up));
        Gizmos.DrawRay(drawRay);
    }
}