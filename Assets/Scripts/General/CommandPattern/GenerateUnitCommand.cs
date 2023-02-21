using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateUnitCommand : ICommand
{
    private readonly GridTile placementTile;
    private readonly PlaceableObject generatedUnit;
    private readonly IUnitGeneration generator;

    public GenerateUnitCommand(GridTile _placementTile, PlaceableObject _generatedUnit, IUnitGeneration generator)
    {
        this.placementTile = _placementTile;
        this.generatedUnit = _generatedUnit;
        this.generator = generator;

        placementTile.SetOccupyingObject(generatedUnit, false);
    }

    public void Execute()
    {
        generator.GenerateUnitAt(placementTile, generatedUnit);
        ICommand.CommandCompleted();
    }

    public void Undo()
    {
        placementTile.SetOccupyingObject(null, false);
        placementTile.UnHighlight();
    }
}
