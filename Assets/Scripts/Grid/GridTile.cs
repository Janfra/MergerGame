using System;
using UnityEngine;

public class GridTile : MonoBehaviour
{
    #region Events

    public Func<GameGrid> OnGetTileGrid;
    public Func<GridTile, GridTile[]> OnGetTileNeighbours;

    #endregion

    #region Variables & Constants

    [Header("Component")]
    [SerializeField] 
    private MeshHighlightHelper highlightHandling;

    [Header("References")]
    [SerializeField] 
    private PlaceableObject occupyingObject;
    public PlaceableObject OccupyingObject => occupyingObject;
    public bool IsOccupied => occupyingObject != null;

    private bool isPlaced = false;
    public bool IsPlaced => isPlaced;

    public bool IsMarked => highlightHandling.IsMarked;

    #endregion

    private void Awake()
    {
        highlightHandling.SetMeshRenderer(GetComponent<MeshRenderer>());
    }

    private void OnMouseEnter()
    {
        highlightHandling.SetToHoverColour();
    }

    private void OnMouseExit()
    {
        if (!IsOccupied && !highlightHandling.IsMarked)
        {
            UnHighlight();
        }
        else if (highlightHandling.IsMarked)
        {
            Highlight();
        }
    }

    #region Highlight

    /// <summary>
    /// Highlights tile
    /// </summary>
    public void Highlight()
    {
        highlightHandling.SetToHighlight();
    }

    /// <summary>
    /// Highlights tile to the set colour.
    /// </summary>
    /// <param name="_newColour">Colour to set tile</param>
    public void Highlight(Color _newColour)
    {
        highlightHandling.SetHighlightColour(_newColour);
        highlightHandling.SetToHighlight();
    }

    /// <summary>
    /// Highlights tile after delay
    /// </summary>
    /// <param name="_newColour">Colour to set tile</param>
    public void HighlightDelayed()
    {
        highlightHandling.SetHighlightAfterDelay();
    }

    /// <summary>
    /// Unhighlights tile
    /// </summary>
    public void UnHighlight()
    {
        if (!IsOccupied)
        {
            highlightHandling.SetBackToDefaultColour();
        }
    }

    /// <summary>
    /// Start timer when finished unhighlights the tile
    /// </summary>
    public void StartHighlightTimer()
    {
        if (!IsOccupied)
        {
            highlightHandling.ColourChangeTimeout();
        }
    }

    #endregion

    #region Occupying Object

    /// <summary>
    /// Changes occupying unit
    /// </summary>
    /// <param name="_newOccupyingObject"></param>
    public void SetOccupyingObject(PlaceableObject _newOccupyingObject, bool _isPlaced = true)
    {
        occupyingObject = _newOccupyingObject;
        isPlaced = _isPlaced;

        if (IsPlaced && _newOccupyingObject != null)
        {
            _newOccupyingObject.OnTileChanged += context => OnObjectTileChanged();
            _newOccupyingObject.SetDefaultPosition(GetObjectPositionOnTile());
        }
        else if(occupyingObject != null)
        {
            Debug.Log("Highlight");
            HighlightDelayed();
        }
    }

    /// <summary>
    /// Returns the position of an object if placed on this tile
    /// </summary>
    /// <returns>Position on top of tile</returns>
    public Vector3 GetObjectPositionOnTile()
    {
        Vector3 objectNewPosition = new(transform.position.x, transform.position.y + GameGrid.TILESIZE, transform.position.z);
        return objectNewPosition;
    }

    /// <summary>
    /// Gets called when object leaves tile
    /// </summary>
    private void OnObjectTileChanged()
    {
        occupyingObject.OnTileChanged -= context => OnObjectTileChanged();
        occupyingObject = null;
        isPlaced = false;
        UnHighlight();
    }

    #endregion

    public GameGrid GetGridOwner()
    {
        return OnGetTileGrid?.Invoke();
    }

    public GridTile[] GetTileNeighbours()
    {
        return OnGetTileNeighbours?.Invoke(this);
    }
}
