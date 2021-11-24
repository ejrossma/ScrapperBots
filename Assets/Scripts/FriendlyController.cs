using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitClass { PALADIN };

public class FriendlyController : MonoBehaviour
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
    public Vector2Int position;

    private BoardManager bm;

    private void Start()
    {
        bm = GameObject.FindGameObjectWithTag("Board").GetComponent<BoardManager>();
    }

    public void MoveToTile(Vector2Int pos)
    {
        Transform newPos = bm.GetTile(pos);
        if(newPos != null)
        {
            transform.position = newPos.position;
            position = pos;
        }    
    }
}
