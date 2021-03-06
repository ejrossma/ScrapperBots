using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction { ABOVE, UPPER_RIGHT, LOWER_RIGHT, BELOW, LOWER_LEFT, UPPER_LEFT };

public class BoardManager : MonoBehaviour
{
    public Material attackIndicatorMat;
    public Material moveIndicatorMat;

    private int rows;
    private int cols;
    private Transform[,] tiles;

    private void Awake()
    {
        rows = transform.childCount;
        cols = transform.GetChild(0).childCount;
        tiles = new Transform[rows, cols];
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                tiles[r, c] = transform.GetChild(r).GetChild(c);
                tiles[r, c].GetComponent<Tile>().position = new Vector2Int(c, r);
                if(c % 2 == 0)
                    tiles[r, c].GetComponent<Tile>().nodePosition = new Vector3Int(c, -((c+1) / 2) + r, -(c / 2) - r);
                else
                    tiles[r, c].GetComponent<Tile>().nodePosition = new Vector3Int(c, -(c / 2) + r, -((c+1) / 2) - r);
            }
        }
    }

    public Transform GetTile(Vector2Int position)
    {
        int row = position.y;
        int col = position.x;
        if (row < 0 || row >= rows || col < 0 || col >= cols)
            return null;
        return tiles[row, col];
    }

    public Transform GetAdjacentTile(Vector2Int position, Direction direction)
    {
        return direction switch
        {
            Direction.ABOVE => GetTile(new Vector2Int(position.x, position.y + 1)),
            Direction.UPPER_RIGHT => GetTile(new Vector2Int(position.x + 1, position.x % 2 == 0 ? position.y : position.y + 1)),
            Direction.LOWER_RIGHT => GetTile(new Vector2Int(position.x + 1, position.x % 2 == 0 ? position.y - 1 : position.y)),
            Direction.BELOW => GetTile(new Vector2Int(position.x, position.y - 1)),
            Direction.LOWER_LEFT => GetTile(new Vector2Int(position.x - 1, position.x % 2 == 0 ? position.y - 1 : position.y)),
            Direction.UPPER_LEFT => GetTile(new Vector2Int(position.x - 1, position.x % 2 == 0 ? position.y : position.y + 1)),
            _ => null
        };
    }

    public void SelectTiles(List<Transform> selectedTiles)
    {
        foreach(Transform t in selectedTiles)
        {
            t.GetChild(0).gameObject.SetActive(true);
            t.GetComponent<Tile>().selected = true;
        }
    }

    public void DeselectTiles()
    {
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                tiles[r, c].GetChild(0).gameObject.SetActive(false);
                tiles[r, c].GetComponent<Tile>().selected = false;
            }
        }
    }

    public void ChangeIndicator(Color col) {
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (col == Color.red) 
                {
                    tiles[r, c].GetChild(0).transform.GetChild(3).GetComponent<MeshRenderer>().material = attackIndicatorMat;
                    tiles[r, c].GetChild(0).transform.GetChild(4).GetComponent<MeshRenderer>().material = attackIndicatorMat;
                    tiles[r, c].GetChild(0).transform.GetChild(5).GetComponent<MeshRenderer>().material = attackIndicatorMat;
                }
                else if (col == Color.blue)
                {
                    tiles[r, c].GetChild(0).transform.GetChild(3).GetComponent<MeshRenderer>().material = moveIndicatorMat;
                    tiles[r, c].GetChild(0).transform.GetChild(4).GetComponent<MeshRenderer>().material = moveIndicatorMat;
                    tiles[r, c].GetChild(0).transform.GetChild(5).GetComponent<MeshRenderer>().material = moveIndicatorMat;                    
                }

            }
        }        
    }

    public bool TileIsMovable(Transform tile)
    {
        return tile.GetComponent<Tile>().tileType == TileType.DEFAULT || tile.GetComponent<Tile>().tileType == TileType.MUD || tile.GetComponent<Tile>().tileType == TileType.WATER || tile.GetComponent<Tile>().tileType == TileType.CHARGED_AIR;
    }
}
