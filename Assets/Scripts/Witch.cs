using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Witch : MonoBehaviour
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
    public void Mesmerize()
    {
        Debug.Log("Witch is using Mesmerize!");
    }

    public void ToggleMesmerizeRange() 
    {

    }

    //basic ability
    public void Corpsecall()
    {
        Debug.Log("Witch is using Corpsecall!");
    }

    public void ToggleCorpsecallRange() 
    {

    }

    //meltdown
    public void Necromancy()
    {
        Debug.Log("Witch has triggered their meltdown and is using Necromancy!");
    }
}
