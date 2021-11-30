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

    // Start is called before the first frame update
    void Start()
    {
        bm = GameObject.FindGameObjectWithTag("Board").GetComponent<BoardManager>();
        sm = GameObject.FindGameObjectWithTag("System Manager").GetComponent<SystemManager>();
        uc = GetComponentInParent<UnitController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //basic ability
        //DIDNT IMPLEMENT NOT MOVING UNTIL END OF NEXT TURN
    public void Mesmerize(UnitController unit)
    {
        Debug.Log("Witch is using Mesmerize on " + unit.unitName + "!");
        mesmerizedUnit = unit;
        selectUnitToMesmerize = true;
        //pan camera to mesmerized unit
        Camera.main.GetComponent<CameraManager>().PanToDestination(new Vector3(unit.transform.position.x, 10, unit.transform.position.z - 4.5f));
        //calculate movement around unit and toggle it
        unit.ToggleMoveRange();
        sm.SelectUnit(mesmerizedUnit);
    }

    //call when player selects movement
    public void MesmerizeMovement(Tile tile) {
        sm.SelectUnit(mesmerizedUnit);
        mesmerizedUnit.BasicMove(tile);
        uc.actionUsed = true;
        mesmerizedUnit = null;
        selectUnitToMesmerize = false;
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
        Debug.Log("Witch is using Corpsecall!");
        uc.actionUsed = true;
        sm.SelectUnit(uc);
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
        Debug.Log("Witch has triggered their meltdown and is using Necromancy!");
    }
}
