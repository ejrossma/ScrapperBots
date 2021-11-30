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
    public int MAXHP;
    public int ATK; //attack
    public int AMR; //armor
    public int MAXAMR;
    public int CRG; //charge
    public int MAXCRG;
    public int SPD; //speed
    public int TRD; //threads
    public int ATKRNG; //attack range
    public Vector2Int position;
    public bool isTurn;
    public bool moving;
    public bool alreadyMoved;
    public bool acting;
    public bool actionUsed;
    public bool heldAction;
    public bool moveRangeShowing;
    public bool abilityOneRangeShowing;
    public bool abilityTwoRangeShowing;
    public bool meltdownRangeShowing;
    public bool attackRangeShowing;
    public bool harvestRangeShowing;
    public Sprite icon;
    public bool isDead;

    //time elasped for Lerp
    public float travelTime;
    //goal time for Lerp
    public float waitTime;

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
    }

    private void Update()
    {

    }

    public void SetTurn()
    {
        isTurn = true;
        moveRangeShowing = false;
        attackRangeShowing = false;
        abilityOneRangeShowing = false;
        abilityTwoRangeShowing = false;
        meltdownRangeShowing = false;
        harvestRangeShowing = false;
        if (isDead)
        {
            StartCoroutine(WaitToEndTurn());
            return;
        }   
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
        bm.DeselectTiles();
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
            abilityOneRangeShowing = false;
            abilityTwoRangeShowing = false;
            meltdownRangeShowing = false;
            harvestRangeShowing = false;
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
            abilityOneRangeShowing = false;
            abilityTwoRangeShowing = false;
            meltdownRangeShowing = false;
            harvestRangeShowing = false;
            bm.DeselectTiles();
            bm.SelectTiles(GetValidAttackPositions());
            bm.ChangeIndicator(Color.red);
        }
        attackRangeShowing = !attackRangeShowing;
    }

    public void BasicMove(Tile tile)
    {
        ToggleMoveRange();
        MoveEffect(tile);
        alreadyMoved = true;
    }

    public void MoveEffect(Tile tile)
    {
        moving = true;
        List<Transform> a = PathfindToTile(tile.position);
        //start lerping
        IEnumerator move = InterpolateUnit(a);
        StopCoroutine(move);
        StartCoroutine(move);
    }

    public void BasicAttack(Vector2Int pos) 
    {
        //get unit at pos
        UnitController unit = sm.GetUnit(pos);
        transform.rotation = CalculateRotation(bm.GetTile(pos));
        StopCoroutine(ResetRotationAfterAttack());
        StartCoroutine(ResetRotationAfterAttack());
        LoseHealth(unit, ATK);
        actionUsed = true;
        ToggleAttackRange();
        sm.SelectUnit(this);
    }

    IEnumerator ResetRotationAfterAttack() 
    {
        
        yield return new WaitForSeconds(0.75f);

        transform.rotation = Quaternion.Euler(0,0,0);

    }

    public void Meltdown()
    {
        EndTurn();
        Die(this);
    }

    public void Die(UnitController unit)
    {
        Debug.Log(unit.unitName + " has died!");
        unit.HP = 0;
        unit.isDead = true;
        bm.GetTile(unit.position).GetComponent<Tile>().ChangeTile(TileType.RUINED_MACHINE);
        sm.activeUnits.Remove(unit.gameObject);
        if(unit.CompareTag("Friendly Unit"))
            sm.friendlyUnits.Remove(unit.gameObject);
        else
            sm.enemyUnits.Remove(unit.gameObject);
        sm.deadUnits.Add(unit.gameObject);
        if(sm.unitTurnOrder.Contains(unit.gameObject))
        {
            sm.unitTurnOrder.Remove(unit.gameObject);
            sm.UpdateTurnOrderDisplay();
        }
        unit.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void Revive(UnitController unit)
    {
        Debug.Log(unitName + " has revived " + unit.unitName + "!");
        //unit.HP = 0;
        //unit.isDead = true;
        //bm.GetTile(unit.position).GetComponent<Tile>().ChangeTile(TileType.RUINED_MACHINE);
        //sm.activeUnits.Remove(unit.gameObject);
        //if (unit.CompareTag("Friendly Unit"))
        //    sm.friendlyUnits.Remove(unit.gameObject);
        //else
        //    sm.enemyUnits.Remove(unit.gameObject);
        //sm.deadUnits.Add(unit.gameObject);
        //if (sm.unitTurnOrder.Contains(unit.gameObject))
        //{
        //    sm.unitTurnOrder.Remove(unit.gameObject);
        //    sm.UpdateTurnOrderDisplay();
        //}
        //unit.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void LoseHealth(UnitController unit, int damage) 
    {
        Debug.Log(unitName + " attacked " + unit.unitName + " for " + damage + "!");
        unit.HP -= damage;
        if (unit.HP <= 0)
        {
            unit.HP = 0;
            Die(unit);
        }
    }

    public void RecoveHealth(UnitController unit, int amount)
    {
        Debug.Log(unit.unitName + " recovered " + amount + " health!");
        unit.HP += amount;
        if (unit.HP > unit.MAXHP)
            unit.HP = unit.MAXHP;
    }

    public void BuffAttack(UnitController unit, int amount)
    {
        Debug.Log(unit.unitName + " gained " + amount + " attack!");
        unit.ATK += amount;
    }

    public void TakeDamage(UnitController unit, int damage)
    {
        if(unit.AMR > 0)
            Debug.Log(unit.unitName + " lost " + damage + " armor!");
        unit.AMR -= damage;
        if (unit.AMR < 0)
        {
            LoseHealth(unit, -unit.AMR);
            unit.AMR = 0;
        }
    }

    public void RecoverArmor(UnitController unit, int amount)
    {
        Debug.Log(unit.unitName + " recovered " + amount + " armor!");
        unit.AMR += amount;
        if (unit.AMR > unit.MAXAMR)
            unit.AMR = unit.MAXAMR;
    }

    public void RecoverCharge(UnitController unit, int amount)
    {
        Debug.Log(unit.unitName + " recovered " + amount + " charge!");
        unit.CRG += amount;
        if (unit.CRG > unit.MAXCRG)
            unit.CRG = unit.MAXCRG;
    }

    public void SpendCharge(UnitController unit, int amount)
    {
        Debug.Log(unit.unitName + " spent " + amount + " charge!");
        unit.CRG -= amount;
    }

    public void MoveToTile(Vector2Int pos)
    {
        Transform newPos = bm.GetTile(pos);
        if (newPos != null)
        {
            position = pos;
        }
    }

    public bool inActionorMovement() 
    {
        return attackRangeShowing || moveRangeShowing || acting || moving;
    }

    IEnumerator InterpolateUnit(List<Transform> tiles)
    {
        foreach (Transform t in tiles) 
        {
            Vector3 start = transform.position;
            //calculate rotation
            transform.rotation = CalculateRotation(t);
            while (travelTime < waitTime)  //condition for interpolation
            {
                transform.position = Vector3.Lerp(start, t.transform.position, (travelTime / waitTime));
                travelTime += Time.deltaTime;
                Camera.main.GetComponent<CameraManager>().PanToDestination(new Vector3(transform.position.x, 10, transform.position.z - 4.5f));
                yield return null;
            }
            travelTime = 0.0f;
            position = t.GetComponent<Tile>().position;
        }
        //End for testing purposes
        //if friendly set y to 0 
            //else set to 180
        transform.rotation = Quaternion.Euler(0,0,0);
        moving = false;
        sm.SelectUnit(this);
    }

    //calculates rotation when given a target tile to look at (can be used for attacks and movement)
    public Quaternion CalculateRotation(Transform tile) 
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

    //calculates direction when given a target tile
    public Direction CalculateDirection(Transform tile) 
    {
        Vector3Int currentTile = bm.GetTile(position).GetComponent<Tile>().nodePosition;
        Vector3Int targetTile = tile.GetComponent<Tile>().nodePosition;

        if (targetTile.y > currentTile.y && targetTile.z < currentTile.z)
            return Direction.ABOVE;
        else if (targetTile.x > currentTile.x && targetTile.z < currentTile.z)
            return Direction.UPPER_RIGHT;
        else if (targetTile.x > currentTile.x && targetTile.y < currentTile.y)
            return Direction.LOWER_RIGHT;
        else if (targetTile.z > currentTile.z && targetTile.y < currentTile.y)
            return Direction.BELOW;
        else if (targetTile.z > currentTile.z && targetTile.x < currentTile.x)
            return Direction.LOWER_LEFT;
        else if (targetTile.y > currentTile.y && targetTile.x < currentTile.x)
            return Direction.UPPER_LEFT;

        Debug.Log("Direction not found!");
        return Direction.ABOVE;
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

    public bool TileOccupied(Transform tile)
    {
        foreach (GameObject unit in sm.activeUnits)
        {
            if (unit.GetComponent<UnitController>().position == tile.GetComponent<Tile>().position)
                return true;
        }
        return false;
    }

    public bool TileOccupiedByTarget(Transform tile)
    {
        if(CompareTag("Friendly Unit"))
        {
            foreach (GameObject unit in sm.enemyUnits)
            {
                if (unit.GetComponent<UnitController>().position == tile.GetComponent<Tile>().position)
                    return true;
            }
        }
        else
        {
            foreach (GameObject unit in sm.friendlyUnits)
            {
                if (unit.GetComponent<UnitController>().position == tile.GetComponent<Tile>().position)
                    return true;
            }
        }
        return false;
    }

    public bool TileOccupiedByFriendly(Transform tile)
    {
        if(CompareTag("Friendly Unit"))
        {
            foreach (GameObject unit in sm.friendlyUnits)
            {
                if (unit.GetComponent<UnitController>().position == tile.GetComponent<Tile>().position)
                    return true;
            }
        }
        else
        {
            foreach (GameObject unit in sm.enemyUnits)
            {
                if (unit.GetComponent<UnitController>().position == tile.GetComponent<Tile>().position)
                    return true;
            }
        }
        return false;
    }

    //based on the attack range return a list of valid attack positions
    public List<Transform> GetValidAttackPositions()
    {
        // Vector3Int fields: X is column, Y is row, Z is length of path
        List<Vector3Int> visited = new List<Vector3Int>();

        CheckForAttack(visited, (Vector3Int)position, Direction.ABOVE);
        CheckForAttack(visited, (Vector3Int)position, Direction.BELOW);
        CheckForAttack(visited, (Vector3Int)position, Direction.UPPER_LEFT);
        CheckForAttack(visited, (Vector3Int)position, Direction.LOWER_LEFT);
        CheckForAttack(visited, (Vector3Int)position, Direction.UPPER_RIGHT);
        CheckForAttack(visited, (Vector3Int)position, Direction.LOWER_RIGHT);

        List<Transform> tiles = new List<Transform>();

        foreach (Vector3Int v in visited)
        {
            if ((Vector2Int)v != position)
                tiles.Add(bm.GetTile((Vector2Int)v));
        }

        return tiles;
    }

    private void CheckForAttack(List<Vector3Int> visited, Vector3Int node, Direction dir)
    {
        if (node.z == ATKRNG + 1)
            return;

        if (TileOccupiedByTarget(bm.GetTile((Vector2Int) node)))
        {
            visited.Add(node);
            return;
        }
        Transform temp = bm.GetAdjacentTile((Vector2Int)node, dir);
        if (temp == null || !bm.TileIsMovable(temp))
            return;
        Vector2Int temp2 = temp.GetComponent<Tile>().position;
        CheckForAttack(visited, new Vector3Int(temp2.x, temp2.y, node.z+1), dir);
    }

    public List<Transform> GetValidAdjacentUnits()
    {
        // Vector3Int fields: X is column, Y is row, Z is length of path
        List<Vector3Int> visited = new List<Vector3Int>();

        CheckForAdjacent(visited, (Vector3Int)position, Direction.ABOVE);
        CheckForAdjacent(visited, (Vector3Int)position, Direction.BELOW);
        CheckForAdjacent(visited, (Vector3Int)position, Direction.UPPER_LEFT);
        CheckForAdjacent(visited, (Vector3Int)position, Direction.LOWER_LEFT);
        CheckForAdjacent(visited, (Vector3Int)position, Direction.UPPER_RIGHT);
        CheckForAdjacent(visited, (Vector3Int)position, Direction.LOWER_RIGHT);

        List<Transform> tiles = new List<Transform>();

        foreach (Vector3Int v in visited)
        {
            if ((Vector2Int)v != position)
                tiles.Add(bm.GetTile((Vector2Int)v));
        }

        return tiles;
    }

    private void CheckForAdjacent(List<Vector3Int> visited, Vector3Int node, Direction dir)
    {
        if (node.z == ATKRNG + 1)
            return;

        if (TileOccupied(bm.GetTile((Vector2Int)node)))
        {
            visited.Add(node);
            return;
        }
        Transform temp = bm.GetAdjacentTile((Vector2Int)node, dir);
        if (temp == null || !bm.TileIsMovable(temp))
            return;
        Vector2Int temp2 = temp.GetComponent<Tile>().position;
        CheckForAttack(visited, new Vector3Int(temp2.x, temp2.y, node.z + 1), dir);
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

    public List<Transform> GetValidHarvestPositions()
    {
        List<Transform> visited = new List<Transform>();
        Transform above = bm.GetAdjacentTile(position, Direction.ABOVE);
        if (above != null && above.GetComponent<Tile>().tileType == TileType.RUINED_MACHINE)
            visited.Add(above);
        Transform upperRight = bm.GetAdjacentTile(position, Direction.UPPER_RIGHT);
        if (upperRight != null && upperRight.GetComponent<Tile>().tileType == TileType.RUINED_MACHINE)
            visited.Add(upperRight);
        Transform lowerRight = bm.GetAdjacentTile(position, Direction.LOWER_RIGHT);
        if (lowerRight != null && lowerRight.GetComponent<Tile>().tileType == TileType.RUINED_MACHINE)
            visited.Add(lowerRight);
        Transform below = bm.GetAdjacentTile(position, Direction.BELOW);
        if (below != null && below.GetComponent<Tile>().tileType == TileType.RUINED_MACHINE)
            visited.Add(below);
        Transform lowerLeft = bm.GetAdjacentTile(position, Direction.LOWER_LEFT);
        if (lowerLeft != null && lowerLeft.GetComponent<Tile>().tileType == TileType.RUINED_MACHINE)
            visited.Add(lowerLeft);
        Transform upperLeft = bm.GetAdjacentTile(position, Direction.UPPER_LEFT);
        if (upperLeft != null && upperLeft.GetComponent<Tile>().tileType == TileType.RUINED_MACHINE)
            visited.Add(upperLeft);
        return visited;
    }

    public void ToggleHarvestRange()
    {
        if (harvestRangeShowing)
        {
            bm.DeselectTiles();
        }
        else
        {
            moveRangeShowing = false;
            attackRangeShowing = false;
            abilityOneRangeShowing = false;
            abilityTwoRangeShowing = false;
            meltdownRangeShowing = false;
            bm.DeselectTiles();
            bm.SelectTiles(GetValidHarvestPositions());
            bm.ChangeIndicator(Color.blue);
        }
        harvestRangeShowing = !harvestRangeShowing;
    }

    public void Harvest(Vector2Int pos)
    {
        Tile t = bm.GetTile(pos).GetComponent<Tile>();
        t.RevertTile();
        RecoverArmor(this, MAXAMR - AMR);
        RecoverCharge(this, MAXCRG - CRG);
        actionUsed = true;
        ToggleHarvestRange();
        sm.SelectUnit(this);
    }
}
