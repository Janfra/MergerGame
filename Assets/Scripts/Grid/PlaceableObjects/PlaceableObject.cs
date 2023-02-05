using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableObject : MonoBehaviour
{
    public static event Action<PlaceableObject, GridTile> OnMoveCommand;
    public event Action OnTileChanged;
    public event Action OnMovementFinished;

    #region Variables

    private Vector3 defaultPosition;
    
    // May not end up being a const to change speed in game
    private const float moveSpeed = 1f;

    #endregion

    /// <summary>
    /// Starts event that clears tile that was occupied.
    /// </summary>
    protected void ClearOccupyingTile()
    {
        OnTileChanged?.Invoke();
    }

    #region Object Placement

    /// <summary>
    /// Creates a movement command to the given tile.
    /// </summary>
    /// <param name="_placementTile">Tile to place object.</param>
    protected void CreateMoveCommand(GridTile _placementTile)
    {
        OnMoveCommand?.Invoke(this, _placementTile);
    }

    /// <summary>
    /// Start animation moving object to tile.
    /// </summary>
    private void MoveToTile()
    {
        if(defaultPosition != transform.position)
        {
            StartCoroutine(MoveUp());
        }
        else
        {
            MovementFinished();
        }
    }

    private IEnumerator MoveUp()
    {
        Vector3 upPosition = transform.position + (Vector3.up * 2);
        float alpha = 0f;

        while (transform.position != upPosition)
        {
            MoveTo(upPosition, ref alpha);
            yield return null;
        }
        StartCoroutine(MoveTowardTile());
    }

    private IEnumerator MoveTowardTile()
    {
        Vector3 targetPosition = new(defaultPosition.x, transform.position.y, defaultPosition.z);
        float alpha = 0f;

        while (transform.position != targetPosition)
        {
            MoveTo(targetPosition, ref alpha);
            yield return null;
        }
        StartCoroutine(MoveDown());
    }

    private IEnumerator MoveDown()
    {
        float alpha = 0f;

        while (transform.position != defaultPosition)
        {
            MoveTo(defaultPosition, ref alpha);
            yield return null;
        }

        Debug.Log("Movement Finished");
        MovementFinished();
    }

    /// <summary>
    /// Lerps object to the given location.
    /// </summary>
    /// <param name="_position">Target location</param>
    /// <param name="alpha">Time/Alpha for the lerp formula</param>
    private void MoveTo(Vector3 _position, ref float alpha)
    {
        alpha += Time.deltaTime * moveSpeed;

        // Multiplied by alpha 3 times for visual effect
        transform.position = Vector3.Lerp(transform.position, _position, Mathf.Clamp01(alpha * alpha * alpha));
    }

    /// <summary>
    /// Starts on movement finished event and clears it.
    /// </summary>
    private void MovementFinished()
    {
        OnMovementFinished?.Invoke();
        // Clear all listeners for reuse
        OnMovementFinished = null;
    }

    /// <summary>
    /// Moves object to tile.
    /// </summary>
    /// <param name="_tile">Tile to place object.</param>
    public void PlaceOnTile(GridTile _tile)
    {
        ClearOccupyingTile();
        _tile.SetOccupyingObject(this);
        Debug.Log($"{_tile.name} is now occupied by {gameObject.name}!");
    }

    /// <summary>
    /// Sets object back to initial position.
    /// </summary>
    protected void GoBackToDefaultPosition()
    {
        transform.position = defaultPosition;
    }

    /// <summary>
    /// Sets new initial position, also sets object to new position.
    /// </summary>
    /// <param name="_defaultPosition"></param>
    /// <param name="_rotation"></param>
    public void SetDefaultPosition(Vector3 _defaultPosition)
    {
        defaultPosition = _defaultPosition;
        MoveToTile();
    }

    #endregion

    #region Generics

    /// <summary>
    /// Check if the given inheriting class is compatible and return it if true.
    /// </summary>
    /// <typeparam name="T">Class to return.</typeparam>
    /// <param name="_convertObject">Object being checked.</param>
    /// <returns>Object as class, otherwise null if not compatible.</returns>
    static public T TryToConvertTo<T>(PlaceableObject _convertObject) where T : PlaceableObject
    {
        T objectToReturn = null;

        if(_convertObject is T)
        {
            objectToReturn = _convertObject as T;
        }

        return objectToReturn;
    }

    #endregion
}
