using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class PlaceableObject : MonoBehaviour
{
    public event Action OnTileChanged;

    [SerializeField] 
    private MouseDragHandler dragHandler;
    [SerializeField] 
    private Vector3 initialPosition;

    private void Awake()
    {
        dragHandler.OnAwake(GameGrid.TILESIZE);
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
        if (_tileFound.IsOccupied)
        {
            GoBackToInitialPosition();
            Debug.Log($"{_tileFound.name} is already taken!");
        }
        else
        {
            OnTileChanged?.Invoke();
            _tileFound.SetOccupyingObject(this);
            Debug.Log($"{_tileFound.name} is now occupied by {gameObject.name}!");
        }
    }

    /// <summary>
    /// Sets object back to initial position
    /// </summary>
    private void GoBackToInitialPosition()
    {
        transform.position = initialPosition;
    }

    /// <summary>
    /// Sets new initial position, also sets object to new position.
    /// </summary>
    /// <param name="_initialPosition"></param>
    /// <param name="_rotation"></param>
    public void SetInitialPosition(Vector3 _initialPosition, Quaternion _rotation)
    {
        initialPosition = _initialPosition;
        transform.SetPositionAndRotation(_initialPosition, _rotation);
    }

    /// <summary>
    /// Error handling for tile not found
    /// </summary>
    private void OnTileNotFound()
    {
        GoBackToInitialPosition();
        Debug.Log("No tile detected!");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Ray drawRay = new Ray(transform.position, transform.TransformDirection(-transform.up));
        Gizmos.DrawRay(drawRay);
    }
}

/// <summary>
/// To handle all drop either for merge or for moving
/// </summary>
public interface IDropable
{
    bool IsDropValid();

    void OnDrop(PlaceableObject _objectPlaced);
}