using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GridTile))]
public class GridTileEvents : MonoBehaviour
{
    [SerializeField]
    private SO_EventHighlightColours onClickHighlightColour;
    private GridTile ownerTile;
    private Action<GridTile> onClicked;

    #region Unity Functions

    private void OnMouseDown()
    {
        onClicked?.Invoke(ownerTile);
    }

    #endregion

    public void ClearOnClick()
    {
        onClicked = null;
    }

    /// <summary>
    /// Binds an action to the run on click event
    /// </summary>
    /// <param name="runOnClick"></param>
    public void SetOnClickedEvent(IOnTileClickableEvent runOnClick)
    {
        onClicked = runOnClick.OnTileSelected;
        ownerTile.Highlight(onClickHighlightColour.eventColour);
    }

    /// <summary>
    /// Sets the owning tile on start
    /// </summary>
    /// <param name="_ownerTile"></param>
    public void SetOwnerTile(GridTile _ownerTile)
    {
        ownerTile = _ownerTile;
    }
}

public interface IOnTileClickableEvent
{
    void OnTileSelected(GridTile _tile);
}
