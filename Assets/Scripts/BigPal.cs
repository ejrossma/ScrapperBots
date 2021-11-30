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

        bool collideWithAlly = GenerateInterceptTiles(visited, uc.position, dir);
        ToggleInterceptRange();
        uc.SpendCharge(uc, 15);
        InterceptMoveEffect(visited, collideWithAlly);
        uc.actionUsed = true;
        //given a chosen direction lerp in that direction until hit a player, wall, edge of map
        //update enemies and players hit depending on what ability says
        //check tile in dir to see if friendly unit stopped the charge
    }

    public void InterceptMoveEffect(List<Vector2Int> visited, bool collideWithAlly)
    {
        uc.moving = true;

        List<Transform> a = new List<Transform>();
        foreach(Vector2Int v in visited)
        {
            a.Add(bm.GetTile(v));
        }
        //start lerping
        IEnumerator move = InterceptInterpolateUnit(a, collideWithAlly);
        StopCoroutine(move);
        StartCoroutine(move);
    }

    IEnumerator InterceptInterpolateUnit(List<Transform> tiles, bool collideWithAlly)
    {
        foreach (Transform t in tiles)
        {
            Vector3 start = transform.position;
            //calculate rotation
            transform.rotation = uc.CalculateRotation(t);
            while (uc.travelTime < uc.waitTime)  //condition for interpolation
            {
                transform.position = Vector3.Lerp(start, t.transform.position, uc.travelTime / uc.waitTime);
                uc.travelTime += Time.deltaTime;
                Camera.main.GetComponent<CameraManager>().PanToDestination(new Vector3(transform.position.x, 10, transform.position.z - 4.5f));
                yield return null;
            }
            uc.travelTime = 0.0f;
            uc.position = t.GetComponent<Tile>().position;
            foreach(GameObject unit in sm.enemyUnits)
            {
                if(unit.GetComponent<UnitController>().position == t.GetComponent<Tile>().position)
                {
                    // Hit enemy
                    uc.LoseHealth(unit.GetComponent<UnitController>(), 10);
                }
            }
        }
        //End for testing purposes
        //if friendly set y to 0 
        //else set to 180
        transform.rotation = Quaternion.Euler(0, 0, 0);
        uc.moving = false;
        if (collideWithAlly)
            uc.RecoverCharge(GetComponent<UnitController>(), 10);
        sm.SelectUnit(uc);
    }

    public void ToggleInterceptRange()
    {
        if (GetComponent<UnitController>().abilityOneRangeShowing) 
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

    private bool GenerateInterceptTiles(List<Vector2Int> visited, Vector2Int node, Direction dir) 
    {
        //return statement
        visited.Add(node);

        Transform tile = bm.GetAdjacentTile(node, dir);
        if (tile == null || !bm.TileIsMovable(tile))
            return false;
        if (uc.TileOccupiedByFriendly(tile))
            return true;
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
