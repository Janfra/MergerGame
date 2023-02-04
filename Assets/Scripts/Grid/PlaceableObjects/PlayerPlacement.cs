using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlacement : PlaceableObject
{
    [SerializeField] 
    private MouseDragHandler dragHandler;

    private void Awake()
    {
        dragHandler.OnAwake(GameGrid.TILESIZE);

        // Temporary default positioning
        SetDefaultPosition(Vector3.zero + Vector3.up);
    }

    private void OnMouseDown()
    {
        GameManager.selectedObject = this;
    }

    private void OnMouseDrag()
    {
        if (!dragHandler.OnDragObjectOnTile(gameObject.transform))
        {
            dragHandler.OnDragObject(gameObject.transform);
            HighlightDropTile();
        }
    }

    private void OnMouseUp()
    {
        DropToTile();
    }

    #region Tile Selection

    /// <summary>
    /// Attempts to return the tile under this object
    /// </summary>
    /// <returns>Returns tile if found, otherwise null</returns>
    private GridTile GetTileUnderObject()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(-transform.up), out RaycastHit hit, Mathf.Infinity))
        {
            if (hit.collider.TryGetComponent(out GridTile selectedTile))
            {
                return selectedTile;
            }
        }
        return null;
    }

    /// <summary>
    /// Highlight tile the object will be dropped on
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
    /// Attempts to drop the object to a tile, if not possible, set back to initial position
    /// </summary>
    private void DropToTile()
    {
        GridTile selectedTile = GetTileUnderObject();
        if (selectedTile)
        {
            OnTileFound(selectedTile);
        }
        else
        {
            OnTileNotFound();
        }
    }

    /// <summary>
    /// Sets tile owner
    /// </summary>
    /// <param name="_tileFound"></param>
    private void OnTileFound(GridTile _tileFound)
    {
        GoBackToDefaultPosition();
        if (_tileFound.IsOccupied)
        {
            Debug.Log($"{_tileFound.name} is already taken!");
        }
        else
        {
            TurnManager.Instance.AddCommand(gameObject, new MoveCommand(this, _tileFound));
        }
    }

    /// <summary>
    /// Error handling for tile not found
    /// </summary>
    private void OnTileNotFound()
    {
        GoBackToDefaultPosition();
        Debug.Log("No tile detected!");
    }

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Ray drawRay = new(transform.position, transform.TransformDirection(-transform.up));
        Gizmos.DrawRay(drawRay);
    }
}