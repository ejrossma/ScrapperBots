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
        // Pause at start of turn to allow player to orient themselves; can adjust later
        yield return new WaitForSeconds(1);

        // Try to move to nearest party member
        if (!uc.alreadyMoved)
        {
            List<List<Transform>> paths = new List<List<Transform>>();
            foreach (GameObject g in sm.friendlyUnits)
            {
                paths.Add(uc.PathfindToTile(g.GetComponent<UnitController>().position));
            }
            paths.Sort((a, b) => a.Count - b.Count);
            paths[0].RemoveAt(0);
            paths[0].RemoveAt(paths[0].Count - 1);
            if (paths[0].Count > 0)
            {
                Debug.Log("Bomberman moving towards target!");
                uc.moving = true;
                StartCoroutine(BombermanMove(paths[0]));
                uc.alreadyMoved = true;
            }
            else
            {
                Debug.Log("Bomberman didn't move towards target!");
            }
        }

        // Try to attack party member
        yield return new WaitUntil(() => !uc.moving);
        if (!uc.actionUsed)
        {
            List<Transform> targets = uc.GetValidAttackPositions();
            if (targets.Count > 0)
            {
                Debug.Log("Bomberman attacked target!");
                BombermanAttack(sm.GetUnit(targets[Random.Range(0, targets.Count - 1)].GetComponent<Tile>().position));
                yield return new WaitForSeconds(1f);
            }
            else
                Debug.Log("Bomberman could not find a target!");
        }

        // End turn
        yield return new WaitForSeconds(1);
        uc.EndTurn();
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
}
