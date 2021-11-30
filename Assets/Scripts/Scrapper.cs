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
        //do damage
        //if kill
            //pop up for instant harvest or using Catch This
        Debug.Log("Scrapper's Teardown");
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
}
