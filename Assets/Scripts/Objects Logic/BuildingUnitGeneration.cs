using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(PlaceableObject))]
public class BuildingUnitGeneration : MonoBehaviour, IUnitGeneration, IUIInteractive, IOnTileClickableEvent
{
    [Header("References")]
    [SerializeField]
    private PlaceableEvents placementEvents;

    [Header("Config")]
    [SerializeField]
    private List<PlaceableObject> unitsAvailable;

    #region Unity Functions

    private void Awake()
    {
        if(placementEvents == null)
        {
            placementEvents = GetComponent<PlaceableEvents>();
        }
    }

    private void OnEnable()
    {
        if (placementEvents == null)
        {
            placementEvents = GetComponent<PlaceableEvents>();
        }

        placementEvents.OnSelected += HighlightValidTiles;
        placementEvents.OnPlacementEvent += BindPlacementCommandGeneration;
    }

    private void OnDisable()
    {
        placementEvents.OnSelected -= HighlightValidTiles;
        placementEvents.OnPlacementEvent -= BindPlacementCommandGeneration;
    }

    #endregion

    private void HighlightValidTiles()
    {
        GridTile[] validTiles = TryGetValidTiles();
        if (validTiles != null)
        {
            foreach (GridTile tile in validTiles)
            {
                placementEvents.AddTileToEvent(tile, this);
            }
        }
    }

    private void BindPlacementCommandGeneration(GridTile _placementTile, PlaceableObject _context2)
    {
        GenerateGenerationCommandAt(_placementTile);
    }

    private void GenerateGenerationCommandAt(GridTile _tile)
    {
        // For now just set
        CreateGenerationCommand(_tile);
        placementEvents.ClearEventTiles();
    }

    private void GenerateUnitOnRandomValidTile()
    {
        GridTile[] spawningTiles = TryGetValidTiles();
        if (spawningTiles.Length > 0)
        {
            int randomTileIndex = Random.Range(0, spawningTiles.Length);
            CreateGenerationCommand(spawningTiles[randomTileIndex]);
        }
    }

    private void CreateGenerationCommand(GridTile _tile)
    {
        IUnitGeneration.UnitGenerationInfo generationInfo = new(GetRandomUnit(), _tile, this, gameObject);
        IUnitGeneration.UnitGenerationCommand(generationInfo);
    }

    private PlaceableObject GetRandomUnit()
    {
        int randomUnitIndex = Random.Range(0, unitsAvailable.Count);
        return unitsAvailable[randomUnitIndex];
    }

    private GridTile[] TryGetValidTiles()
    {
        if (placementEvents.ObjectTile == null)
        {
            Debug.Log("No tile set to event");
            return null;
        }

        GridTile[] neighbours = placementEvents.ObjectTile.GetTileNeighbours();
        List<GridTile> validTiles = new List<GridTile>();

        foreach (GridTile tile in neighbours)
        {
            if (!tile.IsOccupied)
            {
                validTiles.Add(tile);
            }
        }

        return validTiles.ToArray();
    }

    #region IUnitGeneration

    public void GenerateUnitAt(GridTile _tile, PlaceableObject _generatedUnit)
    {
        Instantiate(_generatedUnit).PlaceOnTile(_tile);
    }

    #endregion

    #region IUIInteractive

    public void OnInteracted()
    {
        GenerateUnitOnRandomValidTile();
    }

    #endregion

    #region IOnTileClickable

    public void OnTileSelected(GridTile _tile)
    {
        GenerateGenerationCommandAt(_tile);
    }

    #endregion
}
