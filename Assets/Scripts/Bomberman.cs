using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomberman : MonoBehaviour
{
    private BoardManager bm;
    private SystemManager sm;
    private UnitController uc;

    void Start()
    {
        bm = GameObject.FindGameObjectWithTag("Board").GetComponent<BoardManager>();
        sm = GameObject.FindGameObjectWithTag("System Manager").GetComponent<SystemManager>();
        uc = GetComponentInParent<UnitController>();
    }

    public void RunAI()
    {
        StartCoroutine(BombermanBrain());
    }

    IEnumerator BombermanBrain()
    {
        bool suicided = false;
        // Pause at start of turn to allow player to orient themselves; can adjust later
        yield return new WaitForSeconds(1);

        // Try to commit ultimate sin
        if(!uc.actionUsed)
        {
            int temp = uc.ATKRNG;
            uc.ATKRNG = 1;
            List<Transform> adjacentTargets = uc.GetValidAttackPositions();
            uc.ATKRNG = temp;
            if (adjacentTargets.Count > 1)
            {
                sm.LogMessage("Bomberman commits the ultimate sin!");
                BombermanSelfDestruct(adjacentTargets);
                suicided = true;
            }
        }
        if (!suicided)
        {
            // Try to move to nearest party member
            if (!uc.alreadyMoved)
            {
                List<Transform> spaces = uc.GetValidMovePositions();
                List<Vector3Int> tiles = new List<Vector3Int>();
                foreach (Transform t in spaces)
                {
                    tiles.Add((Vector3Int)t.GetComponent<Tile>().position);
                }
                // Grant values to tiles based on conditions
                for (int i = 0; i < tiles.Count; i++)
                {
                    bool hasLineOfSight = false;
                    foreach (GameObject target in sm.friendlyUnits)
                    {
                        int distance = target.GetComponent<UnitController>().PathfindToTile((Vector2Int)tiles[i]).Count - 1;
                        if (HasLineOfSight((Vector2Int)tiles[i], target.GetComponent<UnitController>()))
                        {
                            if (!hasLineOfSight)
                            {
                                AddScore(tiles, i, 5);
                                hasLineOfSight = true;
                            }
                            else
                            {
                                AddScore(tiles, i, 1);
                            }
                            if (distance == 3)
                            {
                                AddScore(tiles, i, 3);
                            }
                        }
                        if (distance > 3)
                        {
                            AddScore(tiles, i, 1);
                        }
                    }
                }
                tiles.Sort((a, b) => b.z - a.z);
                List<Transform> path = uc.PathfindToTile((Vector2Int)tiles[0]);
                path.RemoveAt(0);
                uc.moving = true;
                StartCoroutine(BombermanMove(path));
            }

            // Try to attack party member
            yield return new WaitUntil(() => !uc.moving);
            if (!uc.actionUsed)
            {
                List<Transform> targets = uc.GetValidAttackPositions();
                if (targets.Count > 0)
                {
                    //sm.LogMessage("Bomberman attacked target!");
                    BombermanAttack(sm.GetUnit(targets[Random.Range(0, targets.Count - 1)].GetComponent<Tile>().position));
                    yield return new WaitForSeconds(1f);
                }
                //else
                //    sm.LogMessage("Bomberman could not find a target!");
            }

            // End turn
            yield return new WaitForSeconds(1);
            uc.EndTurn();
        }
    }

    IEnumerator BombermanMove(List<Transform> tiles)
    {
        for (int i = 0; i < tiles.Count && i < uc.SPD; i++)
        {
            Vector3 start = transform.position;
            //calculate rotation
            transform.rotation = uc.CalculateRotation(tiles[i]);
            while (uc.travelTime < uc.waitTime)  //condition for interpolation
            {
                transform.position = Vector3.Lerp(start, tiles[i].transform.position, uc.travelTime / uc.waitTime);
                uc.travelTime += Time.deltaTime;
                Camera.main.GetComponent<CameraManager>().PanToDestination(new Vector3(transform.position.x, 10, transform.position.z - 4.5f));
                yield return null;
            }
            uc.travelTime = 0.0f;
            uc.position = tiles[i].GetComponent<Tile>().position;
        }
        transform.rotation = Quaternion.Euler(0, 0, 0);
        uc.moving = false;
        sm.SelectUnit(uc);
    }

    private void AddScore(List<Vector3Int> tiles, int id, int amount)
    {
        tiles[id] = new Vector3Int(tiles[id].x, tiles[id].y, tiles[id].z + amount);
    }

    public bool HasLineOfSight(Vector2Int tile, UnitController target)
    {
        Direction direction = uc.InvertDirection(target.CalculateDirection(bm.GetTile(tile)));
        return CheckDirection(uc.ATKRNG, tile, direction, target);
    }

    private bool CheckDirection(float distance, Vector2Int node, Direction dir, UnitController target)
    {
        if (distance == 0)
            return false;
        Transform tile = bm.GetAdjacentTile(node, dir);
        if (tile != null && target.position == tile.GetComponent<Tile>().position)
            return true;
        if (tile == null || !bm.TileIsMovable(tile) || uc.TileOccupied(tile))
            return false;
        return CheckDirection(distance - 1, tile.GetComponent<Tile>().position, dir, target);
    }

    public void BombermanAttack(UnitController unit)
    {
        //get unit at pos
        transform.rotation = uc.CalculateRotation(bm.GetTile(unit.position));
        StopCoroutine(uc.ResetRotationAfterAttack());
        StartCoroutine(uc.ResetRotationAfterAttack());
        uc.TakeDamage(unit, uc.ATK);
        uc.actionUsed = true;
        sm.SelectUnit(uc);
    }

    // Bomberman fucking dies
    public void BombermanSelfDestruct(List<Transform> tiles)
    {
        foreach (Transform tile in tiles)
        {
            UnitController unit = sm.GetUnit(tile.GetComponent<Tile>().position);
            uc.TakeDamage(unit.GetComponent<UnitController>(), 30);
        }
        uc.actionUsed = true;
        uc.Meltdown();
    }
}
