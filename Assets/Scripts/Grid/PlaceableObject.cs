using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableObject : MonoBehaviour
{
    [SerializeField] private MouseDragHandler dragHandler;
    [SerializeField] private Vector3 initialPosition;
    public event Action OnTileChanged;

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
        dragHandler.OnDragObjectOnTile(gameObject.transform);
    }

    private void OnMouseUp()
    {
        DropToTile();
    }

    /// <summary>
    /// Tries to find a tile under the current object to set it as its new position
    /// </summary>
    private void DropToTile()
    {
        // Raycast downwards checking for tiles
        if (Physics.Raycast(transform.position, transform.TransformDirection(-transform.up), out RaycastHit hit, Mathf.Infinity))
        {
            if (hit.collider.TryGetComponent(out GridTile selectedTile))
            {
                OnTileFound(selectedTile);
            }
            else
            {
                OnTileNotFound();
            }
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
        if (_tileFound.IsOccupied())
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