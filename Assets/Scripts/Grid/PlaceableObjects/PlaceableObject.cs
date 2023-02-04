using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableObject : MonoBehaviour
{
    public event Action OnTileChanged;

    #region Variables

    private Vector3 defaultPosition;
    private float moveSpeed = 1f;
    protected Action onMovementFinished;

    #endregion

    protected void TileChanged()
    {
        OnTileChanged?.Invoke();
    }

    #region Object Placement

    private void MoveToTile()
    {
        StartCoroutine(MoveUp());
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
        Vector3 targetPosition = new Vector3(defaultPosition.x, transform.position.y, defaultPosition.z);
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

        onMovementFinished?.Invoke();
    }

    private void MoveTo(Vector3 _position, ref float alpha)
    {
        alpha += Time.deltaTime * moveSpeed;

        // Multiplied by alpha 3 times for visual effect
        transform.position = Vector3.Lerp(transform.position, _position, Mathf.Clamp01(alpha * alpha * alpha));
    }

    /// <summary>
    /// Moves object to tile
    /// </summary>
    /// <param name="_tile"></param>
    public void PlaceOnTile(GridTile _tile)
    {
        TileChanged();
        _tile.SetOccupyingObject(this);
        Debug.Log($"{_tile.name} is now occupied by {gameObject.name}!");
    }

    /// <summary>
    /// Sets object back to initial position
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
}
