using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Witch : MonoBehaviour
{
    private BoardManager bm;
    private SystemManager sm;
    private UnitController uc;

    public bool selectUnitToMesmerize;
    public UnitController mesmerizedUnit;

    void Start()
    {
        bm = GameObject.FindGameObjectWithTag("Board").GetComponent<BoardManager>();
        sm = GameObject.FindGameObjectWithTag("System Manager").GetComponent<SystemManager>();
        uc = GetComponentInParent<UnitController>();
    }

    //basic ability
    public void Mesmerize(UnitController unit)
    {
        mesmerizedUnit = unit;
        selectUnitToMesmerize = true;
        bm.DeselectTiles();
        bm.ChangeIndicator(Color.blue);
        bm.SelectTiles(unit.GetValidMovePositions());
        sm.SelectUnit(mesmerizedUnit);
    }

    //call when player selects movement
    public void MesmerizeMovement(Tile tile) {
        mesmerizedUnit.isMesmerized = true;
        MesmerizeMoveEffect(tile);
        uc.actionUsed = true;
        uc.abilitesUsed++;
        sm.LogMessage("Witch used Mesmerize!");
        uc.SpendCharge(uc, 20);
        ToggleMesmerizeRange();
        sm.SelectUnit(mesmerizedUnit);
    }

    public void MesmerizeMoveEffect(Tile tile)
    {
        mesmerizedUnit.moving = true;
        List<Transform> a = mesmerizedUnit.PathfindToTile(tile.position);
        //start lerping
        IEnumerator move = MesmerizeInterpolateUnit(mesmerizedUnit, a);
        StopCoroutine(move);
        StartCoroutine(move);
    }

    IEnumerator MesmerizeInterpolateUnit(UnitController unit, List<Transform> tiles)
    {
        foreach (Transform t in tiles)
        {
            Vector3 start = unit.transform.position;
            //calculate rotation
            unit.transform.rotation = unit.CalculateRotation(t);
            while (unit.travelTime < unit.waitTime)  //condition for interpolation
            {
                unit.transform.position = Vector3.Lerp(start, t.transform.position, unit.travelTime / unit.waitTime);
                unit.travelTime += Time.deltaTime;
                Camera.main.GetComponent<CameraManager>().PanToDestination(new Vector3(unit.transform.position.x, 10, unit.transform.position.z - 4.5f));
                yield return null;
            }
            unit.travelTime = 0.0f;
            unit.position = t.GetComponent<Tile>().position;
        }
        //End for testing purposes
        //if friendly set y to 0 
        //else set to 180
        unit.transform.rotation = Quaternion.Euler(0, 0, 0);
        unit.moving = false;
        sm.SelectUnit(uc);
    }

    public void ToggleMesmerizeRange() 
    {
        if (GetComponent<UnitController>().abilityOneRangeShowing) 
        {
            bm.DeselectTiles();
        }
        else
        {
            uc.moveRangeShowing = false;
            uc.attackRangeShowing = false;
            uc.abilityTwoRangeShowing = false;
            uc.meltdownRangeShowing = false;
            uc.harvestRangeShowing = false;
            selectUnitToMesmerize = false;
            mesmerizedUnit = null;
            bm.DeselectTiles();
            bm.SelectTiles(GetValidMesmerizeRange());
            bm.ChangeIndicator(Color.red);
        }
        GetComponent<UnitController>().abilityOneRangeShowing = !GetComponent<UnitController>().abilityOneRangeShowing;
    }

    //straight line out in all directions until hit edge of map, wall, or ally
    //called when the player clicks on the intercept ability button
    public List<Transform> GetValidMesmerizeRange() 
    {
        // Vector3Int fields: X is column, Y is row, Z is length of path
        List<Vector2Int> visited = new List<Vector2Int>();

        GenerateMesmerizeTiles(visited, uc.position, Direction.ABOVE);
        GenerateMesmerizeTiles(visited, uc.position, Direction.BELOW);
        GenerateMesmerizeTiles(visited, uc.position, Direction.UPPER_LEFT);
        GenerateMesmerizeTiles(visited, uc.position, Direction.LOWER_LEFT);
        GenerateMesmerizeTiles(visited, uc.position, Direction.UPPER_RIGHT);
        GenerateMesmerizeTiles(visited, uc.position, Direction.LOWER_RIGHT);

        List<Transform> tiles = new List<Transform>();

        foreach (Vector2Int v in visited)
        {
            if (v != uc.position)
                tiles.Add(bm.GetTile(v));
        }

        return tiles;
    }

    private bool GenerateMesmerizeTiles(List<Vector2Int> visited, Vector2Int node, Direction dir) 
    {
        Transform tile = bm.GetAdjacentTile(node, dir);
        if (tile == null || !bm.TileIsMovable(tile))
            return false;
        if (uc.TileOccupiedByFriendly(tile) || uc.TileOccupiedByTarget(tile))
        {
            visited.Add(tile.GetComponent<Tile>().position);
            return true;
        }
        return GenerateMesmerizeTiles(visited, tile.GetComponent<Tile>().position, dir);    
    }

    //basic ability
    public void Corpsecall(Tile tile)
    {
        Direction direction = uc.InvertDirection(uc.CalculateDirection(tile.transform));
        uc.SpendCharge(uc, 10);
        MarchCorpseCall(tile, tile, direction);
        ToggleCorpsecallRange();
        uc.actionUsed = true;
        uc.abilitesUsed++;
        sm.LogMessage("Witch used Corpse Call!");
        sm.SelectUnit(uc);
    }

    public void MarchCorpseCall(Tile originTile, Tile tile, Direction direction)
    {
        UnitController unit = sm.GetUnit(tile.position);
        if (unit != null)
        {
            if(unit.CompareTag("Friendly Unit"))
            {
                // Stop an harvest for friendly
                originTile.RevertTile();
                uc.RecoverArmor(unit, unit.MAXAMR - unit.AMR);
                uc.RecoverCharge(unit, unit.MAXCRG - unit.CRG);
                return;
            }
            else
            {
                int atk = originTile.GetGhostAttack();
                if(atk < 0)
                    uc.TakeDamage(unit, uc.ATK);
                else
                    uc.TakeDamage(unit, atk);
            }
        }
        Transform temp = bm.GetAdjacentTile(tile.position, direction);
        if (temp == null)
            return;
        MarchCorpseCall(originTile, temp.GetComponent<Tile>(), direction);
    }

    public void ToggleCorpsecallRange() 
    {
        if (GetComponent<UnitController>().abilityTwoRangeShowing) 
        {
            bm.DeselectTiles();
        }
        else
        {
            uc.moveRangeShowing = false;
            uc.attackRangeShowing = false;
            uc.abilityOneRangeShowing = false;
            uc.meltdownRangeShowing = false;
            uc.harvestRangeShowing = false;
            bm.DeselectTiles();
            bm.SelectTiles(GetValidCorpsecallRange());
            bm.ChangeIndicator(Color.blue);
        }
        GetComponent<UnitController>().abilityTwoRangeShowing = !GetComponent<UnitController>().abilityTwoRangeShowing;
    }

    public List<Transform> GetValidCorpsecallRange() 
    {
        // Vector3Int fields: X is column, Y is row, Z is length of path
        List<Vector2Int> visited = new List<Vector2Int>();

        GenerateCorpsecallTiles(visited, uc.position, Direction.ABOVE);
        GenerateCorpsecallTiles(visited, uc.position, Direction.BELOW);
        GenerateCorpsecallTiles(visited, uc.position, Direction.UPPER_LEFT);
        GenerateCorpsecallTiles(visited, uc.position, Direction.LOWER_LEFT);
        GenerateCorpsecallTiles(visited, uc.position, Direction.UPPER_RIGHT);
        GenerateCorpsecallTiles(visited, uc.position, Direction.LOWER_RIGHT);

        List<Transform> tiles = new List<Transform>();

        foreach (Vector2Int v in visited)
        {
            if (v != uc.position)
                tiles.Add(bm.GetTile(v));
        }
        return tiles;
    }    

    private bool GenerateCorpsecallTiles(List<Vector2Int> visited, Vector2Int node, Direction dir) 
    {
        Transform tile = bm.GetAdjacentTile(node, dir);
        if (tile == null)
            return false;
        if (tile.GetComponent<Tile>().tileType == TileType.RUINED_MACHINE)
        {
            visited.Add(tile.GetComponent<Tile>().position);
            return true;
        }
        return GenerateCorpsecallTiles(visited, tile.GetComponent<Tile>().position, dir);
    }

    //meltdown
    public void Necromancy()
    {
        List<GameObject> revivableAllies = uc.GetRevivableDeadAllies();
        foreach(GameObject g in revivableAllies)
        {
            uc.Revive(g.GetComponent<UnitController>());
        }
        uc.actionUsed = true;
        uc.abilitesUsed++;
        sm.LogMessage("Witch invoked their meltdown: Necromancy!");
        uc.Meltdown();
    }
}
