using System;
using UnityEngine;

[RequireComponent(typeof(GridTileEvents))]
public class GridTile : MonoBehaviour
{
    #region Delegates

    /// <summary>
    /// Invoked when requesting the GameGrid storing this tile.
    /// </summary>
    public Func<GameGrid> OnGetTileGrid;

    /// <summary>
    /// Invoked when requesting neighbours of this tile.
    /// </summary>
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
    private GridTileEvents tileEvents;
    public GridTileEvents TileEvents => tileEvents;

    public bool IsOccupied => occupyingObject != null;

    private bool isPlaced = false;
    public bool IsPlaced => isPlaced;

    public bool IsMarked => highlightHandling.IsMarked;

    #endregion

    private void Awake()
    {
        highlightHandling.SetMeshRenderer(GetComponent<MeshRenderer>());
        tileEvents = GetComponent<GridTileEvents>();
        tileEvents.SetOwnerTile(this);
    }

    private void OnEnable()
    {
        if(tileEvents == null)
        {
            tileEvents = GetComponent<GridTileEvents>();
            tileEvents.SetOwnerTile(this);
        }
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
            _newOccupyingObject.OnTileChanged += BindTileClearing;
            _newOccupyingObject.SetDefaultPosition(GetObjectPositionOnTile());
        }
        else if(occupyingObject != null)
        {
            Debug.Log("Highlight");
            HighlightDelayed();
        }
    }

    /// <summary>
    /// Returns the position of placed object on this tile
    /// </summary>
    /// <returns>Position on top of tile</returns>
    public Vector3 GetObjectPositionOnTile()
    {
        Vector3 objectNewPosition = new(transform.position.x, transform.position.y + GameGrid.TILESIZE, transform.position.z);
        return objectNewPosition;
    }

    /// <summary>
    /// Binds clearing this tile space to event, once it gets called, it removes itself from the event. NOTE: Added since Lambdas does not let me remove event
    /// </summary>
    private void BindTileClearing(GridTile _context1)
    {
        occupyingObject.OnTileChanged -= BindTileClearing;
        OnObjectTileChanged();
    }

    /// <summary>
    /// Gets called when object leaves tile to clear space
    /// </summary>
    private void OnObjectTileChanged()
    {
        occupyingObject = null;
        isPlaced = false;
        UnHighlight();
    }

    #endregion

    #region Getters

    public GameGrid GetGridOwner()
    {
        return OnGetTileGrid?.Invoke();
    }

    public GridTile[] GetTileNeighbours()
    {
        return OnGetTileNeighbours?.Invoke(this);
    }

    #endregion
}
