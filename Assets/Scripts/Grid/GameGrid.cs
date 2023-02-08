using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    #region Variables & Constants

    [Header("Config")]
    [SerializeField] 
    private GridTile tilePrefab;
    [SerializeField] 
    private int width, lenght;
    public int Width => width;
    public int Lenght => lenght;

    private Dictionary<Vector3, GridTile> tiles;
    public const int TILESIZE = 1;

    #endregion

    private void Awake()
    {
        GenerateGrid();
    }

    #region GridGen

    /// <summary>
    /// Creates the a board based on size set.
    /// </summary>
    public void GenerateGrid()
    {
        tiles = new Dictionary<Vector3, GridTile>();

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < lenght; z++)
            {
                GridTile currentTile = GenerateTile(x, z);
                currentTile.OnGetTileGrid += OnGetGrid;
                // currentTile.OnGetTileNeighbours += OnGetTileNeighbours;
            }
        }
    }

    /// <summary>
    /// Spawns a tile based on its position on the board.
    /// </summary>
    /// <param name="x">X position on board.</param>
    /// <param name="z">Y position on board.</param>
    /// <returns>Generated tile</returns>
    private GridTile GenerateTile(int x, int z)
    {
        GridTile spawnedTile;
        Vector3 tilePosition = new Vector3(x, 0, z) + GetGridBottomCorner();

        spawnedTile = Instantiate(tilePrefab, tilePosition, Quaternion.identity);
        spawnedTile.transform.SetParent(transform);
        spawnedTile.name = $"Tile {x} {z}";

        tiles.Add(tilePosition, spawnedTile);
        return spawnedTile;
    }

    /// <summary>
    /// Get the bottom corner of the grid.
    /// </summary>
    /// <returns>World position of the corner of the board</returns>
    private Vector3 GetGridBottomCorner()
    {
        return new Vector3(transform.position.x - width / 2 + TILESIZE / 2, transform.position.y, transform.position.z - lenght / 2 + TILESIZE / 2);
    }

    #endregion

    private GameGrid OnGetGrid()
    {
        return this;
    }

    // TODO
    //private GridTile[] OnGetTileNeighbours(Vector3 _tilePosition)
    //{
    //    return array results around this tile
    //}

    private void OnDrawGizmos()
    {
        Color currentColour = Gizmos.color;
        currentColour.a = 0.5f;
        Gizmos.color = currentColour;
        Gizmos.DrawCube(transform.position, new Vector3(width, TILESIZE / 2, lenght));
    }

    #region if

    //private void GenerateCenterObj()
    //{
    //    GameObject emtpyGO = new GameObject();
    //    emtpyGO.transform.position = new Vector3((width / 2) - tileHalfSize, -4.5f, 1);
    //    emtpyGO.name = "GridCenter";
    //    gridCenter = emtpyGO.transform;
    //}

    #endregion
}
