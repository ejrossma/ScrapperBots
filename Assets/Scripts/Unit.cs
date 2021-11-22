using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "Unit")]
public class Unit : ScriptableObject
{
    public string unitName;
    public string unitClass;
    public int LVL; //level

    public int HP; //hit points
    public int ATK; //attack
    public int AMR; //armor
    public int CRG; //charge
    public int SPD; //speed
    public int TRD; //threads

    public bool friendly; //are they an enemy
}
