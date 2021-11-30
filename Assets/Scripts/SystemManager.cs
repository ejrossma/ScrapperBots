using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SystemManager : MonoBehaviour
{
    [Header("-Units-")]
    public List<GameObject> activeUnits = new List<GameObject>();
    public List<GameObject> deadUnits = new List<GameObject>();
    public List<GameObject> unitTurnOrder = new List<GameObject>();
    public List<GameObject> friendlyUnits;
    public List<GameObject> enemyUnits;
    public Material friendlyUnit;
    public Material enemyUnit;
    public Material friendlySelected;
    public Material enemySelected;

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
    public Text turnMarker;
    public Sprite defaultTurnIcon;
    public GameObject moveButton;
    public GameObject attackButton;
    public GameObject maintainActionButton;
    public GameObject overloadButton;
    public GameObject skillsButton;
    public GameObject harvestButton;
    public GameObject skillsPanel;
    public GameObject secondPanel;
    public GameObject ability1Button;
    public GameObject ability2Button;
    public GameObject closeSkillsPanelButton;

    private int turnCount;
    private BoardManager bm;

    private void Awake()
    {
        GameObject[] units = GameObject.FindGameObjectsWithTag("Friendly Unit");
        foreach (GameObject g in units)
            friendlyUnits.Add(g);
        units = GameObject.FindGameObjectsWithTag("Enemy Unit");
        foreach (GameObject g in units)
            enemyUnits.Add(g);
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
                UnitController activeUnit = unitTurnOrder[0].GetComponent<UnitController>();
                if (unitTurnOrder[0].CompareTag("Friendly Unit"))
                {
                    SelectUnit(activeUnit);
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
                        if (activeUnit.moveRangeShowing)
                            activeUnit.BasicMove(hit.transform.GetComponentInParent<Tile>());
                        else if (activeUnit.attackRangeShowing)
                            activeUnit.BasicAttack(hit.transform.GetComponentInParent<Tile>().position);
                        else if (activeUnit.harvestRangeShowing)
                            activeUnit.Harvest(hit.transform.GetComponentInParent<Tile>().position);
                        else if (activeUnit.abilityOneRangeShowing && activeUnit.unitClass == UnitClass.BIG_PAL)
                            activeUnit.GetComponent<BigPal>().Intercept(activeUnit.CalculateDirection(hit.transform.GetComponentInParent<Tile>().transform));

                    }
                    // Click on unselected Tile
                    else
                    {
                        
                    }
                }
                //if clicked on unit
                else if (hit.transform.GetComponentInParent<UnitController>())
                {
                    if (activeUnit.attackRangeShowing)
                    {
                        // Click on unit with attack range showing
                        if (bm.GetTile(hit.transform.GetComponentInParent<UnitController>().position).GetComponent<Tile>().selected)
                            activeUnit.BasicAttack(hit.transform.GetComponentInParent<UnitController>().position);
                    }
                    else if (activeUnit.meltdownRangeShowing && activeUnit.unitClass == UnitClass.BIG_PAL)
                    {
                        activeUnit.GetComponent<BigPal>().Sacrifice(hit.transform.GetComponentInParent<UnitController>());
                    }
                    else if (activeUnit.abilityOneRangeShowing && activeUnit.unitClass == UnitClass.SCRAPPER)
                    {
                        activeUnit.GetComponent<Scrapper>().Teardown(hit.transform.GetComponentInParent<UnitController>());
                    }
                    else if (!activeUnit.inActionorMovement())
                    {
                        // Click on unit
                        SelectUnit(hit.transform.GetComponentInParent<UnitController>());
                    }
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
        foreach (GameObject g in activeUnits) {
            g.GetComponent<UnitController>().heldAction = false;
            g.GetComponent<UnitController>().alreadyMoved = false;
            g.GetComponent<UnitController>().actionUsed = false;
            unitTurnOrder.Add(g);
        }

        unitTurnOrder.Sort((a, b) => b.GetComponent<UnitController>().TRD - a.GetComponent<UnitController>().TRD);
        turnCount++;
        turnMarker.text = "Turn: " + turnCount;
        UpdateTurnOrderDisplay();
    }

    public void UnitHoldAction(GameObject unit)
    {
        unit.GetComponent<UnitController>().heldAction = true;
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
        if (unit.isDead)
            return;

        foreach(GameObject g in friendlyUnits)
        {
            g.GetComponentInChildren<SkinnedMeshRenderer>().material = friendlyUnit;
        }
        foreach (GameObject g in enemyUnits)
        {
            g.GetComponentInChildren<SkinnedMeshRenderer>().material = enemyUnit;
        }
        if (unit.CompareTag("Friendly Unit"))
        {
            unit.GetComponentInChildren<SkinnedMeshRenderer>().material = friendlySelected;
        }
        else
        {
            unit.GetComponentInChildren<SkinnedMeshRenderer>().material = enemySelected;
        }

        UpdateStats(unit);
        Camera.main.GetComponent<CameraManager>().PanToDestination(new Vector3(unit.gameObject.transform.position.x, 10, unit.gameObject.transform.position.z - 4.5f));
        // Activate turn UI on unit's turn, otherwise deactivate turn UI
        if (unit.isTurn && !unit.moving && !unit.acting)
        {
            //move button
            moveButton.GetComponent<Button>().interactable = !unit.alreadyMoved;
            moveButton.GetComponent<Button>().onClick.RemoveAllListeners();
            moveButton.GetComponent<Button>().onClick.AddListener(() => unit.ToggleMoveRange());

            //attack button
            attackButton.GetComponent<Button>().interactable = !unit.actionUsed && unit.GetValidAttackPositions().Count > 0;
            attackButton.GetComponent<Button>().onClick.RemoveAllListeners();
            attackButton.GetComponent<Button>().onClick.AddListener(() => unit.ToggleAttackRange());

            //harvest button
            harvestButton.GetComponent<Button>().interactable = !unit.actionUsed && unit.GetValidHarvestPositions().Count > 0;
            harvestButton.GetComponent<Button>().onClick.RemoveAllListeners();
            harvestButton.GetComponent<Button>().onClick.AddListener(() => unit.ToggleHarvestRange());

            //hold action button
            maintainActionButton.GetComponent<Button>().interactable = true;
            maintainActionButton.GetComponent<Button>().onClick.RemoveAllListeners();
            if (!unit.heldAction && (!unit.actionUsed || !unit.alreadyMoved)) {
                maintainActionButton.GetComponent<Button>().onClick.AddListener(() => UnitHoldAction(unit.gameObject));
                maintainActionButton.GetComponentInChildren<Text>().text = "Hold Action (Can only be used once)";
            } else {
                maintainActionButton.GetComponentInChildren<Text>().text = "End Turn";
            }
            maintainActionButton.GetComponent<Button>().onClick.AddListener(() => unit.EndTurn());

            //ability button
            skillsButton.GetComponent<Button>().interactable = true;
            skillsButton.GetComponent<Button>().onClick.RemoveAllListeners();
            //need to make a show abilities function and add the UI into game
            skillsButton.GetComponent<Button>().onClick.AddListener(() => ToggleAbilities(unit));

            // FROM OTHER FUNCTION
            closeSkillsPanelButton.GetComponent<Button>().onClick.RemoveAllListeners();
            closeSkillsPanelButton.GetComponent<Button>().onClick.AddListener(() => { skillsPanel.SetActive(!skillsPanel.activeSelf); secondPanel.SetActive(!secondPanel.activeSelf); bm.DeselectTiles(); });
            
            ability1Button.GetComponent<Button>().interactable = !unit.actionUsed;
            ability2Button.GetComponent<Button>().interactable = !unit.actionUsed;
            overloadButton.GetComponent<Button>().interactable = !unit.actionUsed;
        }
        else
        {
            moveButton.GetComponent<Button>().interactable = false;
            attackButton.GetComponent<Button>().interactable = false;
            harvestButton.GetComponent<Button>().interactable = false;
            maintainActionButton.GetComponent<Button>().interactable = false;
            overloadButton.GetComponent<Button>().interactable = false;

            ability1Button.GetComponent<Button>().interactable = false;
            ability2Button.GetComponent<Button>().interactable = false;
        }

        ability1Button.GetComponent<Button>().onClick.RemoveAllListeners();
        ability2Button.GetComponent<Button>().onClick.RemoveAllListeners();
        overloadButton.GetComponent<Button>().onClick.RemoveAllListeners();
        switch (unit.unitClass)
        {
            case UnitClass.BIG_PAL:
                ability1Button.GetComponentInChildren<Text>().text = "Intercept";
                ability1Button.GetComponent<Button>().onClick.AddListener(() => unit.GetComponent<BigPal>().ToggleInterceptRange()); //ability call here
                // Set false if not enough resource to use
                if (ability1Button.GetComponent<Button>().interactable && (unit.CRG < 15 || unit.GetComponent<BigPal>().GetValidInterceptionRange().Count == 0))
                    ability1Button.GetComponent<Button>().interactable = false;

                ability2Button.GetComponentInChildren<Text>().text = "The Best Defense";
                ability2Button.GetComponent<Button>().onClick.AddListener(() => unit.GetComponent<BigPal>().TheBestDefense()); //ability call here
                // Set false if not enough resource to use
                if (ability2Button.GetComponent<Button>().interactable && (unit.AMR < 40 || unit.GetValidAdjacentUnits().Count == 0))
                    ability2Button.GetComponent<Button>().interactable = false;

                overloadButton.GetComponentInChildren<Text>().text = "Sacrifice (Meltdown)";
                overloadButton.GetComponent<Button>().onClick.AddListener(() => unit.GetComponent<BigPal>().ToggleSacrificeRange()); //ability call here
                break;

            case UnitClass.SCRAPPER:
                ability1Button.GetComponentInChildren<Text>().text = "Teardown"; //ability name needed
                ability1Button.GetComponent<Button>().onClick.AddListener(() => unit.GetComponent<Scrapper>().ToggleTeardownRange()); //ability call here
                // Set false if not enough charge
                //NEED TO GRAY OUT IF THEY CAN'T ATTACK ANYONE
                if (ability1Button.GetComponent<Button>().interactable && (unit.CRG < 40 || unit.GetValidAttackPositions().Count == 0))
                    ability1Button.GetComponent<Button>().interactable = false;

                ability2Button.GetComponentInChildren<Text>().text = "Here, Catch!"; //ability name needed
                //ability2Button.GetComponent<Button>().onClick.AddListener(() => ); //ability call here
                break;
                // case UnitClass.WITCH:
                //     ability1Button.GetComponentInChildren<Text>().text = ; //ability name needed
                //     ability1Button.GetComponent<Button>().onClick.AddListener(() => ); //ability call here

                //     ability2Button.GetComponentInChildren<Text>().text = ; //ability name needed
                //     ability2Button.GetComponent<Button>().onClick.AddListener(() => ); //ability call here
                //     break;
                // case UnitClass.ELECTROMANCER:
                //     ability1Button.GetComponentInChildren<Text>().text = ; //ability name needed
                //     ability1Button.GetComponent<Button>().onClick.AddListener(() => ); //ability call here

                //     ability2Button.GetComponentInChildren<Text>().text = ; //ability name needed
                //     ability2Button.GetComponent<Button>().onClick.AddListener(() => ); //ability call here
                //     break;                                                
        }
    }

    //return the unit at a position
    public UnitController GetUnit(Vector2Int pos) {
        foreach (GameObject unit in activeUnits) {
            if (unit.GetComponent<UnitController>().position == pos)
                return unit.GetComponent<UnitController>();
        }
        return null;
    }

    public void SelectUnit(int id)
    {
        if (id < unitTurnOrder.Count && !unitTurnOrder[0].GetComponent<UnitController>().inActionorMovement())
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

    public void UpdateStats(UnitController unit) 
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
    }

    public void ToggleAbilities(UnitController unit) 
    {
        skillsPanel.SetActive(!skillsPanel.activeSelf); secondPanel.SetActive(!secondPanel.activeSelf);
    }
}
