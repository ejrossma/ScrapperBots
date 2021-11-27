using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitClass { BIG_PAL, SCRAPPER, WITCH, ELECTROMANCER, KNIFE, BOMBERMAN, REAVER, SLUGGER, TANK };

public class UnitController : MonoBehaviour
{
    public string unitName;
    public UnitClass unitClass;
    public int LVL; //level
    public int HP; //hit points
    public int ATK; //attack
    public int AMR; //armor
    public int CRG; //charge
    public int SPD; //speed
    public int TRD; //threads
    public Vector2Int position;
    public bool isTurn;
    public bool alreadyMoved;
    public bool moveRangeShowing;
    public Sprite icon;

    private BoardManager bm;
    private SystemManager sm;

    private void Start()
    {
        bm = GameObject.FindGameObjectWithTag("Board").GetComponent<BoardManager>();
        sm = GameObject.FindGameObjectWithTag("System Manager").GetComponent<SystemManager>();
        MoveToTile(position);
        if (true)
        {
            List<Transform> a = PathfindToTile(new Vector2Int(0, 0));
            Debug.Log("Pathfinding from: " + position);
            foreach (Transform t in a)
            {
                Debug.Log(t.GetComponent<Tile>().position);
            }
        }
    }

    private void Update()
    {
        
    }

    public void SetTurn()
    {
        isTurn = true;
        alreadyMoved = false;
        moveRangeShowing = false;
        if (CompareTag("Enemy Unit"))
        {
            // Pass turn for now
            StartCoroutine(WaitToEndTurn());
        }
        else
        {
            
        }
    }

    IEnumerator WaitToEndTurn()
    {
        yield return new WaitForEndOfFrame();
        EndTurn();
    }

    public void EndTurn()
    {
        isTurn = false;
        sm.AdvanceTurnOrder();
    }

    public void ToggleMoveRange()
    {
        if(moveRangeShowing)
        {
            bm.DeselectTiles();
        }
        else
        {
            bm.SelectTiles(GetValidMovePositions());
        }
        moveRangeShowing = !moveRangeShowing;
    }

    public void BasicMove(Tile tile)
    {
        MoveToTile(tile.position);
        ToggleMoveRange();
        alreadyMoved = true;
        sm.SelectUnit(this);
        // For Testing purposes end turn after move
        EndTurn();
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
        foreach (GameObject unit in sm.activeUnits)
        {
            if (unit.GetComponent<UnitController>().position == tile.GetComponent<Tile>().position)
                return true;
        }
        return false;
    }

    // Algorithm derived from Michal Magdziarz's website https://blog.theknightsofunity.com/pathfinding-on-a-hexagonal-grid-a-algorithm/
    public List<Transform> PathfindToTile(Vector2Int pos)
    {
        List<Tile> openPathTiles = new List<Tile>();
        List<Tile> closedPathTiles = new List<Tile>();

        // Prepare the start tile.
        Tile startPoint = bm.GetTile(position).GetComponent<Tile>();
        Tile endPoint = bm.GetTile(pos).GetComponent<Tile>();
        Tile currentTile = startPoint;

        currentTile.parent = null;
        currentTile.g = 0;
        currentTile.h = GetH(startPoint.nodePosition, endPoint.nodePosition);

        // Add the start tile to the open list.
        openPathTiles.Add(currentTile);

        while (openPathTiles.Count != 0)
        {
            // Sorting the open list to get the tile with the lowest F.
            openPathTiles.Sort((a,b) => (a.g + a.h) - (b.g + b.h));
            currentTile = openPathTiles[0];

            // Removing the current tile from the open list and adding it to the closed list.
            openPathTiles.Remove(currentTile);
            closedPathTiles.Add(currentTile);

            int g = currentTile.g + 1;

            // If there is a target tile in the closed list, we have found a path.
            if (closedPathTiles.Contains(endPoint))
            {
                break;
            }

            CheckAdjacents(currentTile, bm.GetAdjacentTile(currentTile.position, Direction.ABOVE), openPathTiles, closedPathTiles, endPoint, g);
            CheckAdjacents(currentTile, bm.GetAdjacentTile(currentTile.position, Direction.UPPER_RIGHT), openPathTiles, closedPathTiles, endPoint, g);
            CheckAdjacents(currentTile, bm.GetAdjacentTile(currentTile.position, Direction.LOWER_RIGHT), openPathTiles, closedPathTiles, endPoint, g);
            CheckAdjacents(currentTile, bm.GetAdjacentTile(currentTile.position, Direction.BELOW), openPathTiles, closedPathTiles, endPoint, g);
            CheckAdjacents(currentTile, bm.GetAdjacentTile(currentTile.position, Direction.LOWER_LEFT), openPathTiles, closedPathTiles, endPoint, g);
            CheckAdjacents(currentTile, bm.GetAdjacentTile(currentTile.position, Direction.UPPER_LEFT), openPathTiles, closedPathTiles, endPoint, g);
        }

        // Backtracking - setting the final path.
        List<Transform> tiles = new List<Transform>();
        if (closedPathTiles.Contains(endPoint))
        {
            Tile current = endPoint;
            int counter = 500;
            while(current != null && counter > 0)
            {
                tiles.Add(bm.GetTile(current.position));
                current = current.parent;
                counter--;
            }
            tiles.Reverse();
        }

        return tiles;
    }

    private void CheckAdjacents(Tile parent, Transform node, List<Tile> openPathTiles, List<Tile> closedPathTiles, Tile endPoint, int g)
    {
        if (node == null)
            return;
        Tile adjacentTile = node.GetComponent<Tile>();
        // Check if tile is valid
        if (node != null && bm.TileIsMovable(node) && !TileOccupied(node) && !closedPathTiles.Contains(adjacentTile))
        {
            if (!openPathTiles.Contains(adjacentTile))
            {
                adjacentTile.g = g;
                adjacentTile.h = GetH(adjacentTile.nodePosition, endPoint.nodePosition);
                adjacentTile.parent = parent;
                openPathTiles.Add(adjacentTile);
            }
            // Otherwise check if using current G we can get a lower value of F, if so update it's value.
            else if (adjacentTile.g + adjacentTile.h > g + adjacentTile.h)
            {
                adjacentTile.g = g;
                adjacentTile.parent = parent;
            }
        }
    }

    private int GetH(Vector3Int t1, Vector3Int t2)
    {
        return Mathf.Max(Mathf.Abs(t1.x - t2.x), Mathf.Abs(t1.y - t2.y), Mathf.Abs(t1.z - t2.z));
    }
}
