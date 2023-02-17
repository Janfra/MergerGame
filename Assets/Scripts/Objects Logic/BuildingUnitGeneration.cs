using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(PlaceableObject))]
public class BuildingUnitGeneration : MonoBehaviour, IUnitGeneration, IUIInteractive
{
    [Header("Config")]
    [SerializeField]
    private List<PlaceableObject> unitsAvailable;
    private GridTile tile;

    private void Awake()
    {
        SetupTileGetting();
    }

    private void OnEnable()
    {
        SetupTileGetting();
    }

    private void GenerateUnitOnRandomValidTile()
    {
        GridTile[] spawningTiles = TryGetValidTiles();
        if (spawningTiles.Length > 0)
        {
            int randomTileIndex = Random.Range(0, spawningTiles.Length);
            int randomUnitIndex = Random.Range(0, unitsAvailable.Count);

            IUnitGeneration.UnitGenerationInfo generationInfo = new(unitsAvailable[randomUnitIndex], spawningTiles[randomTileIndex], this, gameObject);
            IUnitGeneration.UnitGenerationCommand(generationInfo);
        }
    }

    private GridTile[] TryGetValidTiles()
    {
        if (tile == null)
        {
            Debug.LogError($"No tile set to {gameObject.name} in BuildingUnitGeneration");
            return null;
        }

        GridTile[] neighbours = tile.GetTileNeighbours();
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

    private void SetupTileGetting()
    {
        if (TryGetComponent(out PlaceableObject buildingPlacement))
        {
            buildingPlacement.OnTileChanged += SetBuildingTile;
        }
    }

    public void SetBuildingTile(GridTile _tile)
    {
        tile = _tile;
    }

    public void GenerateUnitAt(GridTile _tile, PlaceableObject _generatedUnit)
    {
        Instantiate(_generatedUnit).PlaceOnTile(_tile);
    }

    public void OnInteracted()
    {
        GenerateUnitOnRandomValidTile();
    }
}
