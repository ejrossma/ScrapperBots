using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scrapper : MonoBehaviour
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

    public void Teardown(UnitController unit)
    {
        Debug.Log("Scrapper's Teardown"); 
        uc.SpendCharge(uc, 40);
        uc.actionUsed = true;
        transform.rotation = uc.CalculateRotation(bm.GetTile(unit.position));
        ToggleTeardownRange();
        sm.ToggleAbilities(uc);
        //do damage
        uc.LoseHealth(unit, uc.ATK);
        StopCoroutine(ResetRotationAfterAttack());
        StartCoroutine(ResetRotationAfterAttack());
        if (unit.HP == 0)
        {
            //NEED TO IMPLEMENT POP UP
                //CHOOSE ONE:
                //HARVEST OR Here, Catch! (can only use here catch if CRG > 10)
            Debug.Log(unit.unitName + " crumbled into scrap!");
        }
    }

    public void ToggleTeardownRange()
    {
        if (uc.abilityOneRangeShowing) 
        {
            bm.DeselectTiles();
        }
        else
        {
            uc.attackRangeShowing = false;
            uc.moveRangeShowing = false;
            uc.abilityTwoRangeShowing = false;
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
}
