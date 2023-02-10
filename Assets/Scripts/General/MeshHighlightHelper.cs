using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MeshHighlightHelper
{
    [Header("References")]
    [SerializeField]
    private MeshRenderer meshRenderer;
    [SerializeField]
    private MonoBehaviour owner;
    
    [Header("Component")]
    [SerializeField]
    private Timer colourTimeoutTimer;

    [Header("Config")]
    [SerializeField]
    private Color defaultColour = Color.white;
    [SerializeField]
    private Color hoverHighlightColour = Color.green;
    [SerializeField]
    private Color highlightColour = Color.red;

    private bool isMarked = false;
    public bool IsMarked => isMarked;

    private const float HIGHLIGHT_TIMER_TIME = 0.1f;

    public MeshHighlightHelper(MeshRenderer _meshRenderer, MonoBehaviour _owner)
    {
        meshRenderer = _meshRenderer;
        colourTimeoutTimer = new Timer();
        owner = _owner;
    }

    /// <summary>
    /// Sets mesh to change colour of.
    /// </summary>
    /// <param name="_meshRenderer"></param>
    public void SetMeshRenderer(MeshRenderer _meshRenderer)
    {
        meshRenderer = _meshRenderer;
    }

    /// <summary>
    /// Sets highlight colour.
    /// </summary>
    /// <param name="_newColour"></param>
    public void SetHighlightColour(Color _newColour)
    {
        highlightColour = _newColour;
    }

    /// <summary>
    /// Changes the colour of the tile mesh.
    /// </summary>
    /// <param name="_newColour">Colour to be set.</param>
    private void ChangeMeshColour(Color _newColour)
    {
        meshRenderer.material.color = _newColour;
    }

    /// <summary>
    /// Start timer when finished set mesh colour to default.
    /// </summary>
    public void ColourChangeTimeout()
    {
        if (colourTimeoutTimer.IsTimerDone)
        {
            SetToHighlight();
            colourTimeoutTimer.SetTimer(HIGHLIGHT_TIMER_TIME, SetBackToDefaultColour);
            colourTimeoutTimer.StartTimer(owner);
        }

        colourTimeoutTimer.SetTimer(colourTimeoutTimer.CurrentTime + Time.deltaTime, false);
    }

    /// <summary>
    /// Sets colour back to default.
    /// </summary>
    public void SetBackToDefaultColour()
    {
        ChangeMeshColour(defaultColour);
        isMarked = false;
    }

    /// <summary>
    /// Sets colour to hover highlight.
    /// </summary>
    public void SetToHoverColour()
    {
        ChangeMeshColour(hoverHighlightColour);
    }

    /// <summary>
    /// Sets colour to highlight.
    /// </summary>
    public void SetToHighlight()
    {
        ChangeMeshColour(highlightColour);
        isMarked = true;
    }

    /// <summary>
    /// Highlight after a small delay. Avoids unhighlighting because of unocuppied for a brief moment.
    /// </summary>
    public void SetHighlightAfterDelay()
    {
        colourTimeoutTimer.SetTimer(HIGHLIGHT_TIMER_TIME, SetToHighlight);
        colourTimeoutTimer.StartTimer(owner);
    }
}