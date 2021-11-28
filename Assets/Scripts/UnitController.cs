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
    public int ATKRNG; //attack range
    public Vector2Int position;
    public bool isTurn;
    public bool moving;
    public bool alreadyMoved;
    public bool acting;
    public bool actionUsed;
    public bool moveRangeShowing;
    public bool attackRangeShowing;
    public Sprite icon;

    //time elasped for Lerp
    private float travelTime;
    //goal time for Lerp
    private float waitTime;

    private BoardManager bm;
    private SystemManager sm;

    private void Start()
    {
        //need to change so it goes on and off when the actions are playing out
        acting = false;

        //interpolation rate for movement of units
        travelTime = 0.0f;
        waitTime = 0.5f;
        bm = GameObject.FindGameObjectWithTag("Board").GetComponent<BoardManager>();
        sm = GameObject.FindGameObjectWithTag("System Manager").GetComponent<SystemManager>();
        MoveToTile(position);
        // if (true)
        // {
        //     List<Transform> a = PathfindToTile(new Vector2Int(0, 0));
        //     Debug.Log("Pathfinding from: " + position);
        //     foreach (Transform t in a)
        //     {
        //         Debug.Log(t.GetComponent<Tile>().position);
        //     }
        // }   
    }

    private void Update()
    {

    }

    public void SetTurn()
    {
        isTurn = true;
        alreadyMoved = false;
        actionUsed = false;
        moveRangeShowing = false;
        attackRangeShowing = false;
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
        if (moveRangeShowing)
        {
            bm.DeselectTiles();
        }
        else
        {
            attackRangeShowing = false;
            bm.DeselectTiles();
            bm.SelectTiles(GetValidMovePositions());
            bm.ChangeIndicator(Color.blue);
        }
        moveRangeShowing = !moveRangeShowing;
    }

    public void ToggleAttackRange()
    {
        if (attackRangeShowing) 
        {
            bm.DeselectTiles();
        }
        else
        {
            moveRangeShowing = false;
            bm.DeselectTiles();
            bm.SelectTiles(GetValidAttackPositions());
            bm.ChangeIndicator(Color.red);
        }
        attackRangeShowing = !attackRangeShowing;
    }

    public void BasicMove(Tile tile)
    {
        ToggleMoveRange();
        moving = true;
        List<Transform> a = PathfindToTile(tile.position);
        //start lerping
        IEnumerator move = InterpolateUnit(a);
        StopCoroutine(move);
        StartCoroutine(move);
        alreadyMoved = true;
        sm.SelectUnit(this);
    }

    public void BasicAttack(Tile tile) 
    {
        Debug.Log("Attack");
        actionUsed = true;
        sm.SelectUnit(this);
    }

    public void MoveToTile(Vector2Int pos)
    {
        Transform newPos = bm.GetTile(pos);
        if (newPos != null)
        {
            position = pos;
        }
    }

    public bool inActionorMovement() {
        return attackRangeShowing || moveRangeShowing || acting || moving;
    }

    IEnumerator InterpolateUnit(List<Transform> tiles)
    {
        foreach (Transform t in tiles) 
        {
            Vector3 start = transform.position;
            //calculate rotation
            transform.rotation = calculateRotation(t);
            while (travelTime < waitTime)  //condition for interpolation
            {
                transform.position = Vector3.Lerp(start, t.transform.position, (travelTime / waitTime));
                travelTime += Time.deltaTime;
                yield return null;
            }
            travelTime = 0.0f;
            position = t.GetComponent<Tile>().position;
            Camera.main.GetComponent<CameraManager>().PanToDestination(new Vector3(t.transform.position.x, 10, t.transform.position.z - 4.5f));

        }
        //End for testing purposes
        //if friendly set y to 0 
            //else set to 180
        transform.rotation = Quaternion.Euler(0,0,0);
        moving = false;
    }

    //calculates rotation when given a target tile to look at (can be used for attacks and movement)
    private Quaternion calculateRotation(Transform tile) 
    {
        //when x is odd lower is in same row
        //when x is even higher is in same row

        //if 1 row to the left
            //upper left (300 degrees)
            //lower left (240 degrees)
        //if same row
            //forward (0 degrees)
            //backward (180 degrees)
        //if 1 row to the right
            //upper right (60 degrees)
            //lower right (120 degrees)

        float rotValue = 0.0f;
        Vector2Int t = tile.GetComponent<Tile>().position;
        if ((position.x % 2 != 0 && position.x > t.x && position.y < t.y) || (position.x % 2 == 0 && position.x > t.x && position.y == t.y)) //upper left
            rotValue = 300.0f;
        else if ((position.x % 2 != 0 && position.x > t.x && position.y == t.y) || (position.x % 2 == 0 && position.x > t.x && position.y > t.y))  //lower left
            rotValue = 240.0f;
        else if (position.x == t.x && position.y < t.y)  //forward
            rotValue = 0.0f;
        else if (position.x == t.x && position.y > t.y)  //backward
            rotValue = 180.0f;
        else if ((position.x % 2 != 0 && position.x < t.x && position.y < t.y) || (position.x % 2 == 0 && position.x < t.x && position.y == t.y))  //upper right
            rotValue = 60.0f;
        else if ((position.x % 2 != 0 && position.x < t.x && position.y == t.y) || (position.x % 2 == 0 && position.x < t.x && position.y > t.y))  //lower right
            rotValue = 120.0f;
        
        return Quaternion.Euler(0, rotValue, 0);
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

    //based on the attack range return a list of valid attack positions
    public List<Transform> GetValidAttackPositions()
    {
        // Vector3Int fields: X is column, Y is row, Z is length of path
        List<Vector3Int> visited = new List<Vector3Int>();

        CheckForAttack(visited, new Vector3Int(position.x, position.y, 0));

        List<Transform> tiles = new List<Transform>();

        foreach (Vector3Int v in visited)
        {
            if ((Vector2Int)v != position)
                tiles.Add(bm.GetTile((Vector2Int)v));
        }

        return tiles;
    }

    private void CheckForAttack(List<Vector3Int> visited, Vector3Int node)
    {
        visited.Add(node);

        if (node.z == ATKRNG)
            return;

        Transform above = bm.GetAdjacentTile((Vector2Int)node, Direction.ABOVE);

        // Check if tile is valid
        if (above != null && bm.TileIsMovable(above) && TileOccupied(above))
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
                CheckForAttack(visited, new Vector3Int(pos.x, pos.y, node.z + 1));
            }
        }

        Transform upperRight = bm.GetAdjacentTile((Vector2Int)node, Direction.UPPER_RIGHT);

        // Check if tile is valid
        if (upperRight != null && bm.TileIsMovable(upperRight) && TileOccupied(upperRight))
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
                CheckForAttack(visited, new Vector3Int(pos.x, pos.y, node.z + 1));
            }
        }

        Transform lowerRight = bm.GetAdjacentTile((Vector2Int)node, Direction.LOWER_RIGHT);

        // Check if tile is valid
        if (lowerRight != null && bm.TileIsMovable(lowerRight) && TileOccupied(lowerRight))
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
                CheckForAttack(visited, new Vector3Int(pos.x, pos.y, node.z + 1));
            }
        }

        Transform below = bm.GetAdjacentTile((Vector2Int)node, Direction.BELOW);

        // Check if tile is valid
        if (below != null && bm.TileIsMovable(below) && TileOccupied(below))
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
                CheckForAttack(visited, new Vector3Int(pos.x, pos.y, node.z + 1));
            }
        }

        Transform lowerLeft = bm.GetAdjacentTile((Vector2Int)node, Direction.LOWER_LEFT);

        // Check if tile is valid
        if (lowerLeft != null && bm.TileIsMovable(lowerLeft) && TileOccupied(lowerLeft))
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
                CheckForAttack(visited, new Vector3Int(pos.x, pos.y, node.z + 1));
            }
        }

        Transform upperLeft = bm.GetAdjacentTile((Vector2Int)node, Direction.UPPER_LEFT);

        // Check if tile is valid
        if (upperLeft != null && bm.TileIsMovable(upperLeft) && TileOccupied(upperLeft))
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
                CheckForAttack(visited, new Vector3Int(pos.x, pos.y, node.z + 1));
            }
        }
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
