using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType { DEFAULT, WALL, RUINED_MACHINE, MUD, WATER, CHARGED_AIR };

public class Tile : MonoBehaviour
{
    public Vector2Int position;
    public TileType tileType;
    public bool selected;
}
