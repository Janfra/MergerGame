using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnitGeneration
{
    public static event Action<UnitGenerationInfo> OnUnitGenerationCommand;

    protected static void UnitGenerationCommand(UnitGenerationInfo _generationInformation)
    {
        OnUnitGenerationCommand?.Invoke(_generationInformation);
    }

    void GenerateUnitAt(GridTile _tile, PlaceableObject _generatedUnit);

    public struct UnitGenerationInfo
    {
        public PlaceableObject GeneratedUnit;
        public GridTile PlacementTile;
        public IUnitGeneration Generator;
        public GameObject GeneratorGameObject;

        public UnitGenerationInfo(PlaceableObject _generatedUnit, GridTile _placementTile, IUnitGeneration _generator, GameObject _generatorGameObject)
        {
            GeneratedUnit = _generatedUnit;
            PlacementTile = _placementTile;
            Generator = _generator;
            GeneratorGameObject = _generatorGameObject;
        }
    }
}
