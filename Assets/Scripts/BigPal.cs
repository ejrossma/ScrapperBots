using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigPal : MonoBehaviour
{
    private BoardManager bm;
    private SystemManager sm;
    private UnitController uc;

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
    public void Intercept(Direction dir) 
    {
        List<Vector2Int> visited = new List<Vector2Int>();

        Transform target = GenerateInterceptTiles(visited, uc.position, dir);
        Debug.Log(dir);
        Debug.Log(target.GetComponent<Tile>().position);
        uc.abilityOneRangeShowing = false;
        bm.DeselectTiles();
        //given a chosen direction lerp in that direction until hit a player, wall, edge of map
        //update enemies and players hit depending on what ability says
        //check tile in dir to see if friendly unit stopped the charge
    }

    public void ToggleInterceptRange()
    {
        if (GetComponent<UnitController>().attackRangeShowing) 
        {
            bm.DeselectTiles();
        }
        else
        {
            GetComponent<UnitController>().moveRangeShowing = false;
            bm.DeselectTiles();
            bm.SelectTiles(GetValidInterceptionRange());
            bm.ChangeIndicator(Color.blue);
        }
        GetComponent<UnitController>().abilityOneRangeShowing = !GetComponent<UnitController>().abilityOneRangeShowing;
    }

    //straight line out in all directions until hit edge of map, wall, or ally
    //called when the player clicks on the intercept ability button
    //NEEDS TO BE TESTED
    public List<Transform> GetValidInterceptionRange() 
    {
        // Vector3Int fields: X is column, Y is row, Z is length of path
        List<Vector2Int> visited = new List<Vector2Int>();

        GenerateInterceptTiles(visited, uc.position, Direction.ABOVE);
        GenerateInterceptTiles(visited, uc.position, Direction.BELOW);
        GenerateInterceptTiles(visited, uc.position, Direction.UPPER_LEFT);
        GenerateInterceptTiles(visited, uc.position, Direction.LOWER_LEFT);
        GenerateInterceptTiles(visited, uc.position, Direction.UPPER_RIGHT);
        GenerateInterceptTiles(visited, uc.position, Direction.LOWER_RIGHT);

        List<Transform> tiles = new List<Transform>();

        foreach (Vector2Int v in visited)
        {
            if (v != uc.position)
                tiles.Add(bm.GetTile(v));
        }

        return tiles;
    }

    private Transform GenerateInterceptTiles(List<Vector2Int> visited, Vector2Int node, Direction dir) 
    {
        //return statement
        visited.Add(node);

        Transform tile = bm.GetAdjacentTile(node, dir);
        if (tile == null || !bm.TileIsMovable(tile) || uc.TileOccupiedByFriendly(tile))
            return bm.GetTile(node);
        return GenerateInterceptTiles(visited, tile.GetComponent<Tile>().position, dir);    
    }

    //basic ability
    public void TheBestDefense() 
    {

    }

    public void GetValidBestDefenseRange() 
    {
        Debug.Log("Best Defense Used");
    }

    //meltdown ability
    public void Sacrifice() 
    {

    }
}
