using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SystemManager : MonoBehaviour
{
    public List<GameObject> activeUnits = new List<GameObject>();
    public GameObject[] friendlyUnits;
    public GameObject[] enemyUnits;

    public GameObject[] turnSlots;

    public int turn;

    void Awake()
    {
        friendlyUnits = GameObject.FindGameObjectsWithTag("Friendly Unit");
        enemyUnits = GameObject.FindGameObjectsWithTag("Enemy Unit");
        GetActiveUnits();
        SetInitiativeOrder();
    }

    private void GetActiveUnits()
    {
        foreach (GameObject g in friendlyUnits)
            activeUnits.Add(g);
        foreach (GameObject g in enemyUnits)
            activeUnits.Add(g);
    }

    private void SetInitiativeOrder()
    {
        activeUnits.Sort((a, b) => b.GetComponent<UnitController>().TRD - a.GetComponent<UnitController>().TRD);
        for(int i = 0; i < turnSlots.Length; i++)
        {
            turnSlots[i].GetComponent<Image>().sprite = activeUnits[(i + turn) % activeUnits.Count].GetComponent<UnitController>().icon;
            turnSlots[i].transform.GetChild(0).GetComponent<Text>().text = activeUnits[(i + turn) % activeUnits.Count].GetComponent<UnitController>().unitName;
        }
    }
}
