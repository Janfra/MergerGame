using UnityEngine;

public class GridTile : MonoBehaviour
{
    [Header("Config")]
    [SerializeField]
    private Color defaultColour = Color.white;
    [SerializeField]
    private Color highlightColour = Color.green;

    [Header("Component")]
    [SerializeField] 
    private Timer highlightTimer;

    [Header("References")]
    [SerializeField] 
    private PlaceableObject occupyingObject;
    public PlaceableObject OccupyingObject => occupyingObject;
    [SerializeField] 
    private MeshRenderer meshRenderer;

    private const float HIGHLIGHT_TIMER_TIME = 0.1f;
    public bool IsOccupied => occupyingObject != null;
    private bool isPlaced = false;
    public bool IsPlaced => isPlaced;

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
        if (!IsOccupied)
        {
            UnHighlight();
        }
    }

    #region Highlight

    /// <summary>
    /// Highlights tile
    /// </summary>
    public void Highlight()
    {
        ChangeMeshColour(highlightColour);
    }

    /// <summary>
    /// Unhighlights tile
    /// </summary>
    public void UnHighlight()
    {
        if (!IsOccupied)
        {
            ChangeMeshColour(defaultColour);
        }
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
    /// Start timer when finished unhighlights the til
    /// </summary>
    public void StartHighlightTimer()
    {
        if (!IsOccupied)
        {
            if (highlightTimer.IsTimerDone)
            {
                Highlight();
                highlightTimer.SetTimer(HIGHLIGHT_TIMER_TIME, UnHighlight);
                highlightTimer.StartTimer(this);
            }

            highlightTimer.SetTimer(highlightTimer.CurrentTime + Time.deltaTime, false);
        }
    }

    /// <summary>
    /// Highlight after a small delay. Avoids unhighlighting because of unocuppied for a brief moment.
    /// </summary>
    public void HighlightDelayed()
    {
        highlightTimer.SetTimer(HIGHLIGHT_TIMER_TIME, Highlight);
        highlightTimer.StartTimer(this);
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

        if (IsPlaced)
        {
            _newOccupyingObject.OnTileChanged += OnObjectTileChanged;
            _newOccupyingObject.SetDefaultPosition(GetObjectPositionOnTile());
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
        occupyingObject.OnTileChanged -= OnObjectTileChanged;
        occupyingObject = null;
        isPlaced = false;
        UnHighlight();
    }

    #endregion
}
