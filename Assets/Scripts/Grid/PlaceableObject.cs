using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableObject : MonoBehaviour
{
    public event Action OnTileChanged;

    [SerializeField] 
    private MouseDragHandler dragHandler;
    private Vector3 defaultPosition;

    private void Awake()
    {
        dragHandler.OnAwake(GameGrid.TILESIZE);

        // Temporary default positioning
        SetDefaultPosition(Vector3.zero + Vector3.up, Quaternion.identity);
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

    #region Object Placement

    /// <summary>
    /// Moves object to tile
    /// </summary>
    /// <param name="_tile"></param>
    public void PlaceOnTile(GridTile _tile)
    {
        OnTileChanged?.Invoke();
        _tile.SetOccupyingObject(this);
        Debug.Log($"{_tile.name} is now occupied by {gameObject.name}!");
    }

    /// <summary>
    /// Sets object back to initial position
    /// </summary>
    private void GoBackToDefaultPosition()
    {
        transform.position = defaultPosition;
    }

    /// <summary>
    /// Sets new initial position, also sets object to new position.
    /// </summary>
    /// <param name="_defaultPosition"></param>
    /// <param name="_rotation"></param>
    public void SetDefaultPosition(Vector3 _defaultPosition, Quaternion _rotation)
    {
        defaultPosition = _defaultPosition;
        transform.SetPositionAndRotation(_defaultPosition, _rotation);
    }

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Ray drawRay = new Ray(transform.position, transform.TransformDirection(-transform.up));
        Gizmos.DrawRay(drawRay);
    }
}