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
        //given a chosen direction lerp in that direction until hit a player, wall, edge of map
        //update enemies and players hit depending on what ability says
        //check tile in dir to see if friendly unit stopped the charge
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

    private void GenerateInterceptTiles(List<Vector2Int> visited, Vector2Int node, Direction dir) 
    {
        //return statement
        Transform tile = bm.GetTile(node);
        if (bm.TileIsMovable(tile) && !uc.TileOccupiedByFriendly(tile))
            visited.Add(node);
        else
            return;

        Transform temp = bm.GetAdjacentTile(node, dir);
        if (temp == null || !bm.TileIsMovable(temp) || uc.TileOccupiedByFriendly(tile))
            return;
        GenerateInterceptTiles(visited, temp.GetComponent<Tile>().position, dir);    
    }

    //basic ability
    public void TheBestDefense() 
    {

    }

    //meltdown ability
    public void Sacrifice() 
    {

    }
}
