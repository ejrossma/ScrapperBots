using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyUnitClass { KNIFE, BOMBERMAN, REAVER, SLUGGER, TANK };

public class EnemyController : MonoBehaviour
{
    public string unitName;
    public EnemyUnitClass unitClass;
    public int LVL; //level
    public int HP; //hit points
    public int ATK; //attack
    public int AMR; //armor
    public int CRG; //charge
    public int SPD; //speed
    public int TRD; //threads
    public Vector2Int position;
    public bool isTurn;

    private BoardManager bm;
    private GameObject[] friendlyUnits;
    private GameObject[] enemyUnits;

    private void Start()
    {
        bm = GameObject.FindGameObjectWithTag("Board").GetComponent<BoardManager>();
        friendlyUnits = GameObject.FindGameObjectsWithTag("Friendly Unit");
        enemyUnits = GameObject.FindGameObjectsWithTag("Enemy Unit");
    }

    private void Update()
    {

    }

    public void MoveToTile(Vector2Int pos)
    {
        Transform newPos = bm.GetTile(pos);
        if (newPos != null)
        {
            transform.position = newPos.position;
            position = pos;
        }
    }

    public List<Transform> GetValidMovePositions()
    {
        // Vector3Int fields: X is column, Y is row, Z is length of path
        List<Vector3Int> visited = new List<Vector3Int>();

        VisitTile(visited, new Vector3Int(position.x, position.y, 0));

        List<Transform> tiles = new List<Transform>();

        foreach (Vector3Int v in visited)
        {
            tiles.Add(bm.GetTile((Vector2Int)v));
        }

        return tiles;
    }

    private void VisitTile(List<Vector3Int> visited, Vector3Int node)
    {
        visited.Add(node);

        if (node.z == SPD)
            return;

        Transform above = bm.GetAdjacentTile((Vector2Int)node, Direction.ABOVE);

        // Check if tile is valid
        if (above != null && bm.TileIsMovable(above) && !TileOccupied(above))
        {
            Vector2Int pos = above.GetComponent<Tile>().position;
            // Check if tile is already in the list (replacing worse case nodes in conflict)
            bool alreadyInList = false;
            for (int i = 0; i < visited.Count; i++)
            {
                if (visited[i].x == pos.x && visited[i].y == pos.y)
                {
                    if (visited[i].z > node.z + 1)
                    {
                        visited.RemoveAt(i);
                    }
                    else
                    {
                        alreadyInList = true;
                    }
                    break;
                }
            }
            // Add tile to visited if not already in list
            if (!alreadyInList)
            {
                VisitTile(visited, new Vector3Int(pos.x, pos.y, node.z + 1));
            }
        }

        Transform upperRight = bm.GetAdjacentTile((Vector2Int)node, Direction.UPPER_RIGHT);

        // Check if tile is valid
        if (upperRight != null && bm.TileIsMovable(upperRight) && !TileOccupied(upperRight))
        {
            Vector2Int pos = upperRight.GetComponent<Tile>().position;
            // Check if tile is already in the list (replacing worse case nodes in conflict)
            bool alreadyInList = false;
            for (int i = 0; i < visited.Count; i++)
            {
                if (visited[i].x == pos.x && visited[i].y == pos.y)
                {
                    if (visited[i].z > node.z + 1)
                    {
                        visited.RemoveAt(i);
                    }
                    else
                    {
                        alreadyInList = true;
                    }
                    break;
                }
            }
            // Add tile to visited if not already in list
            if (!alreadyInList)
            {
                VisitTile(visited, new Vector3Int(pos.x, pos.y, node.z + 1));
            }
        }

        Transform lowerRight = bm.GetAdjacentTile((Vector2Int)node, Direction.LOWER_RIGHT);

        // Check if tile is valid
        if (lowerRight != null && bm.TileIsMovable(lowerRight) && !TileOccupied(lowerRight))
        {
            Vector2Int pos = lowerRight.GetComponent<Tile>().position;
            // Check if tile is already in the list (replacing worse case nodes in conflict)
            bool alreadyInList = false;
            for (int i = 0; i < visited.Count; i++)
            {
                if (visited[i].x == pos.x && visited[i].y == pos.y)
                {
                    if (visited[i].z > node.z + 1)
                    {
                        visited.RemoveAt(i);
                    }
                    else
                    {
                        alreadyInList = true;
                    }
                    break;
                }
            }
            // Add tile to visited if not already in list
            if (!alreadyInList)
            {
                VisitTile(visited, new Vector3Int(pos.x, pos.y, node.z + 1));
            }
        }

        Transform below = bm.GetAdjacentTile((Vector2Int)node, Direction.BELOW);

        // Check if tile is valid
        if (below != null && bm.TileIsMovable(below) && !TileOccupied(below))
        {
            Vector2Int pos = below.GetComponent<Tile>().position;
            // Check if tile is already in the list (replacing worse case nodes in conflict)
            bool alreadyInList = false;
            for (int i = 0; i < visited.Count; i++)
            {
                if (visited[i].x == pos.x && visited[i].y == pos.y)
                {
                    if (visited[i].z > node.z + 1)
                    {
                        visited.RemoveAt(i);
                    }
                    else
                    {
                        alreadyInList = true;
                    }
                    break;
                }
            }
            // Add tile to visited if not already in list
            if (!alreadyInList)
            {
                VisitTile(visited, new Vector3Int(pos.x, pos.y, node.z + 1));
            }
        }

        Transform lowerLeft = bm.GetAdjacentTile((Vector2Int)node, Direction.LOWER_LEFT);

        // Check if tile is valid
        if (lowerLeft != null && bm.TileIsMovable(lowerLeft) && !TileOccupied(lowerLeft))
        {
            Vector2Int pos = lowerLeft.GetComponent<Tile>().position;
            // Check if tile is already in the list (replacing worse case nodes in conflict)
            bool alreadyInList = false;
            for (int i = 0; i < visited.Count; i++)
            {
                if (visited[i].x == pos.x && visited[i].y == pos.y)
                {
                    if (visited[i].z > node.z + 1)
                    {
                        visited.RemoveAt(i);
                    }
                    else
                    {
                        alreadyInList = true;
                    }
                    break;
                }
            }
            // Add tile to visited if not already in list
            if (!alreadyInList)
            {
                VisitTile(visited, new Vector3Int(pos.x, pos.y, node.z + 1));
            }
        }

        Transform upperLeft = bm.GetAdjacentTile((Vector2Int)node, Direction.UPPER_LEFT);

        // Check if tile is valid
        if (upperLeft != null && bm.TileIsMovable(upperLeft) && !TileOccupied(upperLeft))
        {
            Vector2Int pos = upperLeft.GetComponent<Tile>().position;
            // Check if tile is already in the list (replacing worse case nodes in conflict)
            bool alreadyInList = false;
            for (int i = 0; i < visited.Count; i++)
            {
                if (visited[i].x == pos.x && visited[i].y == pos.y)
                {
                    if (visited[i].z > node.z + 1)
                    {
                        visited.RemoveAt(i);
                    }
                    else
                    {
                        alreadyInList = true;
                    }
                    break;
                }
            }
            // Add tile to visited if not already in list
            if (!alreadyInList)
            {
                VisitTile(visited, new Vector3Int(pos.x, pos.y, node.z + 1));
            }
        }
    }

    private bool TileOccupied(Transform tile)
    {
        foreach (GameObject unit in friendlyUnits)
        {
            if (unit.GetComponent<FriendlyController>().position == tile.GetComponent<Tile>().position)
                return true;
        }
        foreach (GameObject unit in enemyUnits)
        {
            if (unit.GetComponent<EnemyController>().position == tile.GetComponent<Tile>().position)
                return true;
        }
        return false;
    }
}
