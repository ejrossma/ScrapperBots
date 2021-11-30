using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType { DEFAULT, WALL, RUINED_MACHINE, MUD, WATER, CHARGED_AIR };

public class Tile : MonoBehaviour
{
    public Vector2Int position;
    public TileType tileType;
    public bool selected;
    public Vector3Int nodePosition;
    public int g;
    public int h;
    public Tile parent;
    private TileType originalType;

    private void Start()
    {
        originalType = tileType;
        transform.GetChild(2).gameObject.SetActive(tileType == TileType.RUINED_MACHINE);
    }

    public void ChangeTile(TileType t)
    {
        tileType = t;
        transform.GetChild(2).gameObject.SetActive(tileType == TileType.RUINED_MACHINE);
    }

    public void RevertTile()
    {
        tileType = originalType;
        transform.GetChild(2).gameObject.SetActive(tileType == TileType.RUINED_MACHINE);
    }
}
