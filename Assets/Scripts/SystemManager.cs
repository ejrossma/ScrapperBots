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
    public GameObject hereCatchPopup;
    public GameObject showDetailedStats;
    public GameObject hideDetailedStats;
    public GameObject bottomUIBar;
    public GameObject helpUI;
    public GameObject sideUIBar;
    public GameObject showCharacterOverview;
    public GameObject hideCharacterOverview;

    public Text topCharacterOverviewNameText;
    public Text topCharacterOverviewArmorText;
    public Text topCharacterOverviewHealthText;
    public Text topCharacterOverviewChargeText;
    public GameObject topCharacterOverviewArmorBar;
    public GameObject topCharacterOverviewHealthBar;
    public GameObject topCharacterOverviewChargeBar;

    public Text middleCharacterOverviewNameText;
    public Text middleCharacterOverviewArmorText;
    public Text middleCharacterOverviewHealthText;
    public Text middleCharacterOverviewChargeText;
    public GameObject middleCharacterOverviewArmorBar;
    public GameObject middleCharacterOverviewHealthBar;
    public GameObject middleCharacterOverviewChargeBar;    

    public Text bottomCharacterOverviewNameText;
    public Text bottomCharacterOverviewArmorText;
    public Text bottomCharacterOverviewHealthText;
    public Text bottomCharacterOverviewChargeText;
    public GameObject bottomCharacterOverviewArmorBar;
    public GameObject bottomCharacterOverviewHealthBar;
    public GameObject bottomCharacterOverviewChargeBar;

    public GameObject resultScreen;

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
        hereCatchPopup.SetActive(false);
        resultScreen.SetActive(false);
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
                        else if (activeUnit.abilityTwoRangeShowing && activeUnit.unitClass == UnitClass.SCRAPPER && activeUnit.GetComponent<Scrapper>().hereCatchPhase == 1)
                            activeUnit.GetComponent<Scrapper>().HereCatchPhase1(hit.transform.GetComponentInParent<Tile>().position);
                        //THIS NEEDS TO BE TESTED
                            //CURRENTLY BREAKS BECAUSE MESMERIZEDUNIT IS NULL SOMETIMES
                            //ALSO NEED TO RESET MESMERIZEDUNIT AND SELECTUNITTOMESMERIZE AFTER USING ABILITY OR AFTER CANCELLING OUT
                        else if (activeUnit.unitClass == UnitClass.WITCH && activeUnit.GetComponent<Witch>().mesmerizedUnit != null && activeUnit.GetComponent<Witch>().selectUnitToMesmerize)
                            activeUnit.GetComponent<Witch>().MesmerizeMovement(hit.transform.GetComponentInParent<Tile>());
                        //THIS MIGHT NEED TO BE FIXED BECAUSE IDK IF THE RUINED MACHINE TILES HAVE HITBOXES ON THE MACHINE PART
                        else if (activeUnit.abilityTwoRangeShowing && activeUnit.unitClass == UnitClass.WITCH)
                            activeUnit.GetComponent<Witch>().Corpsecall(hit.transform.GetComponentInParent<Tile>());

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
                    else if (activeUnit.abilityTwoRangeShowing && activeUnit.unitClass == UnitClass.SCRAPPER && activeUnit.GetComponent<Scrapper>().hereCatchPhase == 2)
                    {
                        activeUnit.GetComponent<Scrapper>().HereCatchPhase2(hit.transform.GetComponentInParent<UnitController>().position);
                    }
                    else if (activeUnit.meltdownRangeShowing && activeUnit.unitClass == UnitClass.SCRAPPER)
                    {
                        activeUnit.GetComponent<Scrapper>().LastHarvest(hit.transform.GetComponentInParent<UnitController>().position);
                    }
                    else if (activeUnit.abilityOneRangeShowing && activeUnit.unitClass == UnitClass.WITCH && !activeUnit.GetComponent<Witch>().selectUnitToMesmerize)
                    {
                        activeUnit.GetComponent<Witch>().Mesmerize(hit.transform.GetComponentInParent<UnitController>());
                    }                    
                    else if (!activeUnit.inActionorMovement())
                    {
                        // Click on unit
                        SelectUnit(hit.transform.GetComponentInParent<UnitController>());
                    }
                }

            }
            else if (!EventSystem.current.IsPointerOverGameObject())
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
        {
            g.GetComponent<UnitController>().heldAction = false;
            g.GetComponent<UnitController>().actionUsed = false;
            g.GetComponent<UnitController>().alreadyMoved = false;
            g.GetComponent<UnitController>().isMesmerized = false;
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
            if (unitTurnOrder.Count > i)
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

        foreach (GameObject g in friendlyUnits)
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
        UpdateCharacterOverview();
        Camera.main.GetComponent<CameraManager>().PanToDestination(new Vector3(unit.gameObject.transform.position.x, 10, unit.gameObject.transform.position.z - 4.5f));
        // Activate turn UI on unit's turn, otherwise deactivate turn UI
        if (unit.isTurn && !unit.moving && !unit.acting && unit.CompareTag("Friendly Unit"))
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
            if (!unit.heldAction && (!unit.actionUsed || !unit.alreadyMoved))
            {
                maintainActionButton.GetComponent<Button>().onClick.AddListener(() => UnitHoldAction(unit.gameObject));
                maintainActionButton.GetComponentInChildren<Text>().text = "Hold Action (Can only be used once)";
                maintainActionButton.tag = "Hold Action";
            }
            else
            {
                maintainActionButton.GetComponentInChildren<Text>().text = "End Turn";
                maintainActionButton.tag = "End Turn";
            }
            maintainActionButton.GetComponent<Button>().onClick.AddListener(() => unit.EndTurn());

            //ability button
            skillsButton.GetComponent<Button>().interactable = true;
            skillsButton.GetComponent<Button>().onClick.RemoveAllListeners();
            //need to make a show abilities function and add the UI into game
            skillsButton.GetComponent<Button>().onClick.AddListener(() => ToggleAbilities(unit));

            // FROM OTHER FUNCTION
            closeSkillsPanelButton.GetComponent<Button>().onClick.RemoveAllListeners();
            closeSkillsPanelButton.GetComponent<Button>().onClick.AddListener(() => {
                unit.moveRangeShowing = false;
                unit.attackRangeShowing = false;
                unit.abilityOneRangeShowing = false;
                unit.meltdownRangeShowing = false;
                unit.harvestRangeShowing = false;
                skillsPanel.SetActive(!skillsPanel.activeSelf);
                secondPanel.SetActive(!secondPanel.activeSelf);
                bm.DeselectTiles();
            });

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
                ability1Button.tag = "Intercept";
                ability1Button.GetComponent<Button>().onClick.AddListener(() => unit.GetComponent<BigPal>().ToggleInterceptRange()); //ability call here
                // Set false if not enough resource to use
                if (ability1Button.GetComponent<Button>().interactable && (unit.CRG < 15 || unit.GetComponent<BigPal>().GetValidInterceptionRange().Count == 0))
                    ability1Button.GetComponent<Button>().interactable = false;

                ability2Button.GetComponentInChildren<Text>().text = "The Best Defense";
                ability2Button.tag = "The Best Defense";
                ability2Button.GetComponent<Button>().onClick.AddListener(() => unit.GetComponent<BigPal>().TheBestDefense()); //ability call here
                // Set false if not enough resource to use
                if (ability2Button.GetComponent<Button>().interactable && (unit.AMR < 40 || unit.GetValidAdjacentUnits().Count == 0))
                    ability2Button.GetComponent<Button>().interactable = false;

                overloadButton.GetComponentInChildren<Text>().text = "Sacrifice (Meltdown)";
                overloadButton.tag = "Sacrifice";
                overloadButton.GetComponent<Button>().onClick.AddListener(() => unit.GetComponent<BigPal>().ToggleSacrificeRange()); //ability call here
                if (overloadButton.GetComponent<Button>().interactable && !unit.AreAliveAllies())
                    overloadButton.GetComponent<Button>().interactable = false;
                break;

            case UnitClass.SCRAPPER:
                ability1Button.GetComponentInChildren<Text>().text = "Teardown"; //ability name needed
                ability1Button.tag = "Teardown";
                ability1Button.GetComponent<Button>().onClick.AddListener(() => unit.GetComponent<Scrapper>().ToggleTeardownRange()); //ability call here
                // Set false if not enough resource to use
                if (ability1Button.GetComponent<Button>().interactable && (unit.CRG < 40 || unit.GetValidAttackPositions().Count == 0))
                    ability1Button.GetComponent<Button>().interactable = false;

                ability2Button.GetComponentInChildren<Text>().text = "Here, Catch!";
                ability2Button.tag = "Here, Catch!";
                ability2Button.GetComponent<Button>().onClick.AddListener(() => unit.GetComponent<Scrapper>().ToggleHereCatchRange()); //ability call here
                // Set false if not enough resource to use
                if (ability2Button.GetComponent<Button>().interactable && (unit.CRG < 10 || unit.GetValidHarvestPositions().Count == 0 || unit.GetComponent<Scrapper>().GetValidHereCatchPositions().Count == 0))
                    ability2Button.GetComponent<Button>().interactable = false;

                overloadButton.GetComponentInChildren<Text>().text = "Last Harvest (Meltdown)";
                overloadButton.tag = "Last Harvest";
                overloadButton.GetComponent<Button>().onClick.AddListener(() => unit.GetComponent<Scrapper>().ToggleLastHarvestRange()); //ability call here
                if (overloadButton.GetComponent<Button>().interactable && unit.GetValidAttackPositions().Count == 0)
                    overloadButton.GetComponent<Button>().interactable = false;
                break;

            case UnitClass.WITCH:
                ability1Button.GetComponentInChildren<Text>().text = "Mesmerize";
                ability1Button.tag = "Mesmerize";
                ability1Button.GetComponent<Button>().onClick.AddListener(() => unit.GetComponent<Witch>().ToggleMesmerizeRange()); //ability call here
                //Set false if not enough resource to use
                if (ability1Button.GetComponent<Button>().interactable && (unit.CRG < 20 || unit.GetComponent<Witch>().GetValidMesmerizeRange().Count == 0))
                    ability1Button.GetComponent<Button>().interactable = false;

                ability2Button.GetComponentInChildren<Text>().text = "Corpsecall";
                ability2Button.tag = "Corpsecall";
                ability2Button.GetComponent<Button>().onClick.AddListener(() => unit.GetComponent<Witch>().ToggleCorpsecallRange()); //ability call here
                // Set false if not enough resource to use
                if (ability2Button.GetComponent<Button>().interactable && (unit.CRG < 10 || unit.GetComponent<Witch>().GetValidCorpsecallRange().Count == 0))
                    ability2Button.GetComponent<Button>().interactable = false;

                overloadButton.GetComponentInChildren<Text>().text = "Necromancy (Meltdown)";
                overloadButton.tag = "Necromancy";
                overloadButton.GetComponent<Button>().onClick.AddListener(() => unit.GetComponent<Witch>().Necromancy()); //ability call here
                if (overloadButton.GetComponent<Button>().interactable && unit.GetRevivableDeadAllies().Count == 0)
                    overloadButton.GetComponent<Button>().interactable = false;
                break;

                // case UnitClass.ELECTROMANCER:
                //     ability1Button.GetComponentInChildren<Text>().text = ; //ability name needed
                //     ability1Button.GetComponent<Button>().onClick.AddListener(() => ); //ability call here

                //     ability2Button.GetComponentInChildren<Text>().text = ; //ability name needed
                //     ability2Button.GetComponent<Button>().onClick.AddListener(() => ); //ability call here
                //     break;                                                
        }
    }

    //return the unit at a position
    public UnitController GetUnit(Vector2Int pos)
    {
        foreach (GameObject unit in activeUnits)
        {
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
        displayBattery.text = "Charge: " + unit.CRG;
        displayMovement.text = "Movement: " + unit.SPD;
        displayThreads.text = "Threads: " + unit.TRD;
        displayAttack.text = "Attack: " + unit.ATK;
        displayBuffs.text = "Attack Range: " + unit.ATKRNG;
        //displayDebuffs.text = "Debuffs:";
    }

    public void UpdateCharacterOverview() 
    {
        int count = 1;
        foreach (GameObject g in friendlyUnits) {
            UpdateCharacterOverviewStats(g.GetComponent<UnitController>(), count);
            count++;
        }
    }

    private void UpdateCharacterOverviewStats(UnitController uc, int count)
    {
        if (count == 1)
        {
            //top of 3
            topCharacterOverviewNameText.text = uc.unitName;
            topCharacterOverviewArmorText.text = uc.AMR + "/" + uc.MAXAMR;
            topCharacterOverviewHealthText.text = uc.HP + "/" + uc.MAXHP;
            topCharacterOverviewChargeText.text = uc.CRG + "/" + uc.MAXCRG;

            topCharacterOverviewArmorBar.GetComponent<Image>().fillAmount = (float)uc.AMR / (float)uc.MAXAMR;
            topCharacterOverviewHealthBar.GetComponent<Image>().fillAmount = (float)uc.HP / (float)uc.MAXHP;
            topCharacterOverviewChargeBar.GetComponent<Image>().fillAmount = (float)uc.CRG / (float)uc.MAXCRG;
        }
        else if (count == 2)
        {
            //middle of 3
            middleCharacterOverviewNameText.text = uc.unitName;
            middleCharacterOverviewArmorText.text = uc.AMR + "/" + uc.MAXAMR;
            middleCharacterOverviewHealthText.text = uc.HP + "/" + uc.MAXHP;
            middleCharacterOverviewChargeText.text = uc.CRG + "/" + uc.MAXCRG;
            
            middleCharacterOverviewArmorBar.GetComponent<Image>().fillAmount = (float)uc.AMR / (float)uc.MAXAMR;
            middleCharacterOverviewHealthBar.GetComponent<Image>().fillAmount = (float)uc.HP / (float)uc.MAXHP;
            middleCharacterOverviewChargeBar.GetComponent<Image>().fillAmount = (float)uc.CRG / (float)uc.MAXCRG;                        
        }
        else if (count == 3)
        {
            //bottom of 3
            bottomCharacterOverviewNameText.text = uc.unitName;
            bottomCharacterOverviewArmorText.text = uc.AMR + "/" + uc.MAXAMR;
            bottomCharacterOverviewHealthText.text = uc.HP + "/" + uc.MAXHP;
            bottomCharacterOverviewChargeText.text = uc.CRG + "/" + uc.MAXCRG;

            bottomCharacterOverviewArmorBar.GetComponent<Image>().fillAmount = (float)uc.AMR / (float)uc.MAXAMR;
            bottomCharacterOverviewHealthBar.GetComponent<Image>().fillAmount = (float)uc.HP / (float)uc.MAXHP;
            bottomCharacterOverviewChargeBar.GetComponent<Image>().fillAmount = (float)uc.CRG / (float)uc.MAXCRG;
        }
    }

    public void ToggleAbilities(UnitController unit)
    {
        skillsPanel.SetActive(!skillsPanel.activeSelf); secondPanel.SetActive(!secondPanel.activeSelf);
    }

    public void ToggleDetailedStats() 
    {
        showDetailedStats.SetActive(!showDetailedStats.activeSelf);
        hideDetailedStats.SetActive(!hideDetailedStats.activeSelf);
        bottomUIBar.SetActive(!bottomUIBar.activeSelf);
    }

    public void ToggleCharacterOverview() 
    {
        showCharacterOverview.SetActive(!showCharacterOverview.activeSelf);
        hideCharacterOverview.SetActive(!hideCharacterOverview.activeSelf);
        sideUIBar.SetActive(!sideUIBar.activeSelf);
    }    

    public void ToggleHelpUI()
    {
        helpUI.SetActive(!helpUI.activeSelf);
    }

    public void GameOver(bool win)
    {
        resultScreen.SetActive(true);
        Transform panel = resultScreen.transform;
        // Overall Result
        panel.GetChild(1).GetComponent<Text>().text = win ? "Victory!" : "Defeat!";
        // Turns Taken
        panel.GetChild(3).GetChild(0).GetComponent<Text>().text = "Turns Taken: " + turnCount;
        // Score
        panel.GetChild(3).GetChild(1).GetComponent<Text>().text = "Score: " + (win ? "Good" : "Bad");
        // Allied Units Lost
        panel.GetChild(4).GetChild(0).GetComponent<Text>().text = "Allied Units Lost: " + DeadAllies();
        // Enemy Units Destroyed
        panel.GetChild(4).GetChild(1).GetComponent<Text>().text = "Enemy Units Destroyed: " + DeadEnemies();

        GameObject[] units = GameObject.FindGameObjectsWithTag("Friendly Unit");
        for(int i = 0; i < 3; i++)
        {
            UnitController unit = units[i].GetComponent<UnitController>();
            // Unit Name
            panel.GetChild(6 + i).GetChild(0).GetComponent<Text>().text = unit.unitName;
            // Enemies Destroyed
            panel.GetChild(6 + i).GetChild(1).GetComponent<Text>().text = "Destroyed: " + unit.destroyed;
            // Damage Dealt
            panel.GetChild(6 + i).GetChild(2).GetComponent<Text>().text = "Damage Dealt: " + unit.damageDealt;
            // Damage Taken
            panel.GetChild(6 + i).GetChild(3).GetComponent<Text>().text = "Damage Taken: " + unit.damageTaken;
            // Abilities Used
            panel.GetChild(6 + i).GetChild(4).GetComponent<Text>().text = "Abilities Used: " + unit.abilitesUsed;
        }
    }

    private int DeadAllies()
    {
        int count = 0;
        foreach(GameObject g in deadUnits)
        {
            if (g.CompareTag("Friendly Unit"))
                count++;
        }
        return count;
    }

    private int DeadEnemies()
    {
        int count = 0;
        foreach (GameObject g in deadUnits)
        {
            if (g.CompareTag("Enemy Unit"))
                count++;
        }
        return count;
    }
}