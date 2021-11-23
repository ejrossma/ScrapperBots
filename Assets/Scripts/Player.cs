using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private PositionManager pm = new PositionManager();
    public GameManager gm;
    
    private bool active;
    public Unit PlayerUnit;


    //On each turn player can
        //move or not move
        //&
        //attack or use an ability or harvest a fallen enemy or meltdown    

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (gm.ActiveUnit.unitName == PlayerUnit.unitName) {
            active = true;
        }
    }

}
