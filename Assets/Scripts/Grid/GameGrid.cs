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

    private readonly Dictionary<int, GridTile> indexToTile = new();
    private readonly Dictionary<GridTile, int> tileToIndex = new();
    public const int TILESIZE = 1;
    private const int MIN_SEARCH_SIZE = 3;

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
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < lenght; z++)
            {
                GridTile currentTile = GenerateTile(x, z);
                SetupTileEvents(currentTile);
                StoreInDictionaries(x, z, currentTile);
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

    /// <summary>
    /// Sets up the tile events.
    /// </summary>
    /// <param name="_tile"></param>
    private void SetupTileEvents(GridTile _tile)
    {
        _tile.OnGetTileGrid += OnGetGrid;
        _tile.OnGetTileNeighbours += OnGetTileNeighbours;
    }

    /// <summary>
    /// Adds to the dictionaries the given parameters for future searching.
    /// </summary>
    /// <param name="_x">X index</param>
    /// <param name="_z">Z index</param>
    /// <param name="_tile">Tile stored</param>
    private void StoreInDictionaries(int _x, int _z, GridTile _tile)
    {
        int tileIndex = ConvertFromXandZToIndex(_x, _z);
        // Debug.Log("Tile index added: " + tileIndex);

        indexToTile.Add(tileIndex, _tile);
        tileToIndex.Add(_tile, tileIndex);
    }

    #endregion

    #region Getters

    private GameGrid OnGetGrid()
    {
        return this;
    }

    private GridTile[] OnGetTileNeighbours(GridTile _tileRequesting)
    {
        return GetTilesOnASquareAroundTarget(_tileRequesting, MIN_SEARCH_SIZE, MIN_SEARCH_SIZE);
    }

    /// <summary>
    /// Gets tiles around the tile by the size given
    /// </summary>
    /// <param name="_tileRequesting">Center of the square to search</param>
    /// <param name="_width">Widht of the square</param>
    /// <param name="_lenght">Lenght of the square</param>
    /// <returns></returns>
    private GridTile[] GetTilesOnASquareAroundTarget(GridTile _tileRequesting, int _width, int _lenght)
    {
        // Index setup
        int originTileIndex = tileToIndex[_tileRequesting];
        int originX = 0;
        int originZ = 0;
        GetXandZByRefFromIndex(originTileIndex, ref originX, ref originZ);

        // Clamping
        _width = Mathf.Clamp(_width, MIN_SEARCH_SIZE, width);
        _lenght = Mathf.Clamp(_lenght, MIN_SEARCH_SIZE, lenght);
        // Square size
        int halfWidht = Mathf.FloorToInt(_width / 2);
        int halfLenght = Mathf.FloorToInt(_lenght / 2);

        // Return values
        List<GridTile> tilesToReturn = new();

        for (int x = originX - halfWidht; x < originX + halfWidht + 1; x++)
        {
            for (int z = originZ - halfLenght; z < originZ + halfLenght + 1; z++)
            {
                if (x == originX && z == originZ)
                    continue;

                int index = ConvertFromXandZToIndex(x, z);
                if (indexToTile.ContainsKey(index))
                {
                    tilesToReturn.Add(indexToTile[index]);
                }
            }
        }

        return tilesToReturn.ToArray();
    }

    /// <summary>
    /// Sets the given values to be the X and Z position of the given index.
    /// </summary>
    /// <param name="index">Index to find position</param>
    /// <param name="_x">Value to set to the X position</param>
    /// <param name="_z">Value to set to the Z position</param>
    private void GetXandZByRefFromIndex(int index, ref int _x, ref int _z)
    {
        _z = (index % lenght);
        _x = (index - _z) / width;
        // Debug.Log($"Origin Index: {index}, x: {_x}, z: {_z}");
    }

    #endregion

    /// <summary>
    /// Gets the index at the given position
    /// </summary>
    /// <param name="_x">X position</param>
    /// <param name="_z">Z position</param>
    /// <returns>Index at X and Z position. -1: Position not valid</returns>
    private int ConvertFromXandZToIndex(int _x, int _z)
    {
        if(_x < 0 || _z < 0 || _x >= width || _z >= lenght)
        {
            return -1;
        }

        int index = (_x * width) + _z;
        return index;
    }

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
