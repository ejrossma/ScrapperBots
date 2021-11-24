using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType { DEFAULT, OBSTACLE };

public class Tile : MonoBehaviour
{
    public Vector2Int position;
    public TileType tileType;
}
