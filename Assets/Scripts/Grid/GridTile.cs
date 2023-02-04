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
    [SerializeField] 
    private MeshRenderer meshRenderer;

    private const float HIGHLIGHT_TIMER_TIME = 0.1f;
    public bool IsOccupied => occupyingObject != null;

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
        ChangeMeshColour(highlightColour);
    }

    /// <summary>
    /// Unhighlights tile
    /// </summary>
    public void UnHighlight()
    {
        ChangeMeshColour(defaultColour);
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
        if (highlightTimer.IsTimerDone)
        {
            Highlight();
            highlightTimer.SetTimer(HIGHLIGHT_TIMER_TIME, UnHighlight);
            highlightTimer.StartTimer(this);
        }

        highlightTimer.SetTimer(highlightTimer.CurrentTime + Time.deltaTime, false);
    }

    #endregion

    #region Occupying Object

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
