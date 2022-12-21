using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTile : MonoBehaviour
{
    [SerializeField] private PlaceableObject occupyingObject;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Timer highlightTimer;
    private const float HIGHLIGHTTIMERTIMER = 0.1f;

    private void Awake()
    {
        if(meshRenderer == null)
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }
    }

    private void OnMouseEnter()
    {
        Highlight();
    }

    private void OnMouseExit()
    {
        UnHighlight();
    }

    #region Highlight

    /// <summary>
    /// Highlights tile
    /// </summary>
    public void Highlight()
    {
        ChangeMeshColour(Color.green);
    }

    /// <summary>
    /// Unhighlights tile
    /// </summary>
    public void UnHighlight()
    {
        ChangeMeshColour(Color.white);
    }

    /// <summary>
    /// Changes the colour of the tile mesh
    /// </summary>
    /// <param name="_newColour">Colour to be set</param>
    private void ChangeMeshColour(Color _newColour)
    {
        meshRenderer.material.color = _newColour;
    }

    /// <summary>
    /// Calls the timeout timer
    /// </summary>
    public void StartHighlightTimer()
    {
        StartCoroutine(HighlightTimeOut());
    }

    /// <summary>
    /// Start timer when finished unhighlights the tile, if called when already started extends timeout time.
    /// </summary>
    /// <returns></returns>
    private IEnumerator HighlightTimeOut()
    {
        highlightTimer.SetTimer(HIGHLIGHTTIMERTIMER + highlightTimer.CurrentTime, false);
        if (highlightTimer.IsTimerDone)
        {
            highlightTimer.StartTimer(this);
            while (!highlightTimer.IsTimerDone)
            {
                yield return null;
            }
            UnHighlight();
        }
        yield return null;
    }

    #endregion

    #region Occupying Object

    /// <summary>
    /// Is the tile already being occupied
    /// </summary>
    /// <returns>Is the tile taken</returns>
    public bool IsOccupied()
    {
        return occupyingObject != null;
    }

    /// <summary>
    /// Changes occupying unit
    /// </summary>
    /// <param name="_newOccupyingObject"></param>
    public void SetOccupyingObject(PlaceableObject _newOccupyingObject)
    {
        occupyingObject = _newOccupyingObject;
        _newOccupyingObject.OnTileChanged += OnObjectTileChanged;

        Vector3 objectNewPosition = new Vector3(transform.position.x, transform.position.y + GameGrid.TILESIZE, transform.position.z);
        _newOccupyingObject.SetInitialPosition(objectNewPosition, transform.rotation);
    }

    /// <summary>
    /// Gets called when object leaves tile
    /// </summary>
    private void OnObjectTileChanged()
    {
        occupyingObject.OnTileChanged -= OnObjectTileChanged;
        occupyingObject = null;
    }

    #endregion
}
