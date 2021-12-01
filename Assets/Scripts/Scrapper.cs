using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scrapper : MonoBehaviour
{
    public int hereCatchPhase;
    public Tile hereCatchTile;

    private BoardManager bm;
    private SystemManager sm;
    private UnitController uc;

    // Start is called before the first frame update
    void Start()
    {
        bm = GameObject.FindGameObjectWithTag("Board").GetComponent<BoardManager>();
        sm = GameObject.FindGameObjectWithTag("System Manager").GetComponent<SystemManager>();
        uc = GetComponentInParent<UnitController>();

        hereCatchPhase = 1;
    }

    public void Teardown(UnitController unit)
    {
        uc.SpendCharge(uc, 40);
        uc.actionUsed = true;
        uc.abilitesUsed++;
        sm.LogMessage("Scrapper used Teardown!");
        transform.rotation = uc.CalculateRotation(bm.GetTile(unit.position));
        ToggleTeardownRange();
        uc.TakeDirectDamage(unit, uc.ATK);
        StopCoroutine(ResetRotationAfterAttack());
        StartCoroutine(ResetRotationAfterAttack());
        sm.SelectUnit(uc);
        if (unit.isDead)
        {
            sm.hereCatchPopup.SetActive(true);

            sm.hereCatchPopup.transform.GetChild(2).GetComponent<Button>().onClick.RemoveAllListeners();
            sm.hereCatchPopup.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => HarvestEffect(unit.position));

            sm.hereCatchPopup.transform.GetChild(3).GetComponent<Button>().interactable = !(uc.CRG < 10 || GetValidHereCatchPositions().Count == 0);
            sm.hereCatchPopup.transform.GetChild(3).GetComponent<Button>().onClick.RemoveAllListeners();
            sm.hereCatchPopup.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(() => HereCatchEffect(unit.position));
        } 
    }

    private void HarvestEffect(Vector2Int pos)
    {
        Tile t = bm.GetTile(pos).GetComponent<Tile>();
        t.RevertTile();
        uc.RecoverArmor(uc, uc.MAXAMR - uc.AMR);
        uc.RecoverCharge(uc, uc.MAXCRG - uc.CRG);
        sm.SelectUnit(uc);
        sm.hereCatchPopup.SetActive(false);
    }

    private void HereCatchEffect(Vector2Int pos)
    {
        sm.hereCatchPopup.SetActive(false);
        uc.abilityTwoRangeShowing = true;
        bm.ChangeIndicator(Color.blue);
        HereCatchPhase1(pos);
    }

    public void ToggleTeardownRange()
    {
        if (uc.abilityOneRangeShowing) 
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
            bm.SelectTiles(uc.GetValidAttackPositions());
            bm.ChangeIndicator(Color.red);
        }
        uc.abilityOneRangeShowing = !uc.abilityOneRangeShowing;
    }

    IEnumerator ResetRotationAfterAttack() 
    {
        yield return new WaitForSeconds(0.75f);
        transform.rotation = Quaternion.Euler(0,0,0);
    }

    public void ToggleHereCatchRange()
    {
        if (uc.abilityTwoRangeShowing)
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
            hereCatchPhase = 1;
            bm.DeselectTiles();
            bm.SelectTiles(uc.GetValidHarvestPositions());
            bm.ChangeIndicator(Color.blue);
        }
        uc.abilityTwoRangeShowing = !uc.abilityTwoRangeShowing;
    }

    public void HereCatchPhase1(Vector2Int pos)
    {
        Tile t = bm.GetTile(pos).GetComponent<Tile>();
        hereCatchTile = t;
        bm.DeselectTiles();
        bm.SelectTiles(GetValidHereCatchPositions());
        hereCatchPhase = 2;
    }

    public List<Transform> GetValidHereCatchPositions()
    {
        // Vector3Int fields: X is column, Y is row, Z is length of path
        List<Vector3Int> visited = new List<Vector3Int>();

        CheckForHereCatch(visited, (Vector3Int)uc.position, Direction.ABOVE);
        CheckForHereCatch(visited, (Vector3Int)uc.position, Direction.BELOW);
        CheckForHereCatch(visited, (Vector3Int)uc.position, Direction.UPPER_LEFT);
        CheckForHereCatch(visited, (Vector3Int)uc.position, Direction.LOWER_LEFT);
        CheckForHereCatch(visited, (Vector3Int)uc.position, Direction.UPPER_RIGHT);
        CheckForHereCatch(visited, (Vector3Int)uc.position, Direction.LOWER_RIGHT);

        List<Transform> tiles = new List<Transform>();

        foreach (Vector3Int v in visited)
        {
            if ((Vector2Int)v != uc.position)
                tiles.Add(bm.GetTile((Vector2Int)v));
        }

        return tiles;
    }

    private void CheckForHereCatch(List<Vector3Int> visited, Vector3Int node, Direction dir)
    {
        if (node.z == 4)
            return;

        if (uc.TileOccupiedByFriendly(bm.GetTile((Vector2Int)node)) && (Vector2Int)node != uc.position)
        {
            visited.Add(node);
            return;
        }
        Transform temp = bm.GetAdjacentTile((Vector2Int)node, dir);
        if (temp == null || !bm.TileIsMovable(temp) || uc.TileOccupiedByTarget(temp))
            return;
        Vector2Int temp2 = temp.GetComponent<Tile>().position;
        CheckForHereCatch(visited, new Vector3Int(temp2.x, temp2.y, node.z + 1), dir);
    }

    public void HereCatchPhase2(Vector2Int pos)
    {
        hereCatchTile.RevertTile();
        hereCatchTile = null;
        UnitController unit = sm.GetUnit(pos);
        uc.RecoverArmor(unit, unit.MAXAMR - unit.AMR);
        uc.RecoverCharge(unit, unit.MAXCRG - unit.CRG);

        transform.rotation = uc.CalculateRotation(bm.GetTile(unit.position));
        StopCoroutine(ResetRotationAfterAttack());
        StartCoroutine(ResetRotationAfterAttack());

        uc.actionUsed = true;
        uc.abilitesUsed++;
        sm.LogMessage("Here, Catch!");
        uc.SpendCharge(uc, 10);
        ToggleHereCatchRange();
        sm.SelectUnit(uc);
    }

    public void ToggleLastHarvestRange()
    {
        if (uc.meltdownRangeShowing)
        {
            bm.DeselectTiles();
        }
        else
        {
            uc.moveRangeShowing = false;
            uc.attackRangeShowing = false;
            uc.abilityOneRangeShowing = false;
            uc.abilityTwoRangeShowing = false;
            uc.harvestRangeShowing = false;
            bm.DeselectTiles();
            bm.SelectTiles(uc.GetValidAttackPositions());
            bm.ChangeIndicator(Color.red);
        }
        uc.meltdownRangeShowing = !uc.meltdownRangeShowing;
    }

    public void LastHarvest(Vector2Int pos)
    {
        UnitController unit = sm.GetUnit(pos);
        ToggleLastHarvestRange();
        uc.Die(unit);

        int changedTiles = 0;
        List<Direction> directions = new List<Direction>() { Direction.ABOVE, Direction.UPPER_RIGHT, Direction.LOWER_RIGHT, Direction.BELOW, Direction.LOWER_LEFT, Direction.LOWER_RIGHT };
        for (int i = 0; i < 6; i++)
        {
            Direction d = GetRandomDirection(directions);
            Transform tile = bm.GetAdjacentTile(pos, d);
            directions.Remove(d);
            if (tile != null && bm.TileIsMovable(tile) && !uc.TileOccupied(tile) && changedTiles < 3)
            {
                tile.GetComponent<Tile>().ChangeTile(TileType.RUINED_MACHINE);
                changedTiles++;
            }
        }

        uc.actionUsed = true;
        uc.abilitesUsed++;
        sm.LogMessage("Scrapper invoked their meltdown: Last Harvest!");
        uc.Meltdown();
    }

    private Direction GetRandomDirection(List<Direction> list)
    {
        return list[Random.Range(0, list.Count - 1)];
    }
}
