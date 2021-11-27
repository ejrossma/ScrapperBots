using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SystemManager : MonoBehaviour
{
    [Header("-Units-")]
    public List<GameObject> activeUnits = new List<GameObject>();
    public List<GameObject> unitTurnOrder = new List<GameObject>();
    public GameObject[] friendlyUnits;
    public GameObject[] enemyUnits;

    [Header("-UI-")]
    public GameObject[] turnSlots;
    public Text displayName;
    public Text displayHealth;
    public Text displayArmor;
    public Text displayBattery;
    public Text displayMovement;
    public Text displayThreads;
    public Text displayAttack;
    public Text displayBuffs;
    public Text displayDebuffs;
    public Sprite defaultTurnIcon;
    public GameObject moveButton;
    public GameObject attackButton;
    public GameObject overloadButton;
    public GameObject skillsButton;
    public GameObject harvestButton;
    public GameObject skillsPanel;
    public GameObject ability1Button;
    public GameObject ability2Button;
    public GameObject closeSkillsPanelButton;

    private BoardManager bm;

    private void Awake()
    {
        friendlyUnits = GameObject.FindGameObjectsWithTag("Friendly Unit");
        enemyUnits = GameObject.FindGameObjectsWithTag("Enemy Unit");
        bm = GameObject.FindGameObjectWithTag("Board").GetComponent<BoardManager>();
        GetActiveUnits();
    }

    private void Start()
    {
        SetTurnOrderRound();
        SetTurn(unitTurnOrder[0]);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && unitTurnOrder.Count > 0)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out hit, 250f))
            {
                if (unitTurnOrder[0].CompareTag("Friendly Unit"))
                {
                    SelectUnit(unitTurnOrder[0].GetComponent<UnitController>());
                } 
                else
                {
                    return;
                }
                if (hit.transform.GetComponentInParent<Tile>())
                {
                    // Click on selected Tile
                    if (hit.transform.GetComponentInParent<Tile>().selected)
                    {
                        unitTurnOrder[0].GetComponent<UnitController>().BasicMove(hit.transform.GetComponentInParent<Tile>());
                    }
                    // Click on unselected Tile
                    else
                    {
                        
                    }
                }
                else if (hit.transform.GetComponentInParent<UnitController>())
                {
                    // Click on unit
                    SelectUnit(hit.transform.GetComponentInParent<UnitController>());
                }
            }
            else if(!EventSystem.current.IsPointerOverGameObject())
            {
                if (unitTurnOrder[0].CompareTag("Friendly Unit"))
                {
                    SelectUnit(unitTurnOrder[0].GetComponent<UnitController>());
                }
                else
                {
                    return;
                }
            }
        }
    }

    private void GetActiveUnits()
    {
        foreach (GameObject g in friendlyUnits)
            activeUnits.Add(g);
        foreach (GameObject g in enemyUnits)
            activeUnits.Add(g);
    }

    private void SetTurnOrderRound()
    {
        foreach (GameObject g in activeUnits)
            unitTurnOrder.Add(g);
        unitTurnOrder.Sort((a, b) => b.GetComponent<UnitController>().TRD - a.GetComponent<UnitController>().TRD);
        UpdateTurnOrderDisplay();
    }

    public void UnitHoldAction(GameObject unit)
    {
        unitTurnOrder.Add(unit);
    }

    public void AdvanceTurnOrder()
    {
        unitTurnOrder.RemoveAt(0);
        if (unitTurnOrder.Count == 0)
            SetTurnOrderRound();
        UpdateTurnOrderDisplay();
        SetTurn(unitTurnOrder[0]);
    }

    public void SetTurn(GameObject unit)
    {
        unit.GetComponent<UnitController>().SetTurn();
        SelectUnit(unit.GetComponent<UnitController>());
    }

    public void UpdateTurnOrderDisplay()
    {
        for (int i = 0; i < turnSlots.Length; i++)
        {
            if(unitTurnOrder.Count > i)
            {
                turnSlots[i].GetComponent<Image>().sprite = unitTurnOrder[i].GetComponent<UnitController>().icon;
                turnSlots[i].transform.GetChild(0).GetComponent<Text>().text = unitTurnOrder[i].GetComponent<UnitController>().unitName;
            }
            else
            {
                turnSlots[i].GetComponent<Image>().sprite = defaultTurnIcon;
                turnSlots[i].transform.GetChild(0).GetComponent<Text>().text = "";
            }
        }
    }

    public void SelectUnit(UnitController unit)
    {
        displayName.text = unit.unitName;
        displayHealth.text = "Health: " + unit.HP;
        displayArmor.text = "Armor: " + unit.AMR;
        displayBattery.text = "Battery: " + unit.CRG;
        displayMovement.text = "Movement: " + unit.SPD;
        displayThreads.text = "Threads: " + unit.TRD;
        displayAttack.text = "Attack: " + unit.ATK;
        displayBuffs.text = "Buffs:";
        displayDebuffs.text = "Debuffs:";
        Camera.main.GetComponent<CameraManager>().PanToDestination(new Vector3(unit.gameObject.transform.position.x, 10, unit.gameObject.transform.position.z - 4.5f));
        // Activate turn UI on unit's turn, otherwise deactivate turn UI
        if(unit.isTurn)
        {
            moveButton.GetComponent<Button>().enabled = !unit.alreadyMoved;
            moveButton.GetComponent<Button>().onClick.RemoveAllListeners();
            moveButton.GetComponent<Button>().onClick.AddListener(() => unit.ToggleMoveRange());
        }
        else
        {
            moveButton.GetComponent<Button>().enabled = false;
        }
    }

    public void SelectUnit(int id)
    {
        if (id < unitTurnOrder.Count)
            SelectUnit(unitTurnOrder[id].GetComponent<UnitController>());
    }

    public void DeselectUnit()
    {
        displayName.text = "<No unit selected>";
        displayHealth.text = "Health: ";
        displayArmor.text = "Armor: ";
        displayBattery.text = "Battery: ";
        displayMovement.text = "Movement: ";
        displayThreads.text = "Threads: ";
        displayAttack.text = "Attack: ";
        displayBuffs.text = "Buffs:";
        displayDebuffs.text = "Debuffs:";
    }
}
