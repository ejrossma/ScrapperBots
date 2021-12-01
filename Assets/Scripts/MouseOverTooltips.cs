using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseOverTooltips : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    Dictionary<string, string> tooltips = new Dictionary<string, string>();
    public GameObject tooltipBox;
    private GameObject anchor;
    private Transform triangle;

    // Start is called before the first frame update
    void Start()
    {
        triangle = tooltipBox.transform.GetChild(0);
        tooltipBox.SetActive(false);
        anchor = GameObject.FindGameObjectWithTag("Tooltip Anchor");

        //left oriented
        tooltips.Add("Health", "Damage taken is dealt to health if armor is 0. A unit dies if its health reaches 0.");
        tooltips.Add("Armor", "Armor takes damage before health. If it reaches 0, damage will be dealt to health. Can be restored by harvesting ruined machines.");
        tooltips.Add("Charge", "Spent to use a unit's abilities. Can be restored by harvesting ruined machines.");
        tooltips.Add("Speed", "How many hexes a unit can move in a turn");

        //middle oriented
        tooltips.Add("Threads", "How early in the turn order a unit will move. More Threads means a unit acts before one with less Threads.");
        tooltips.Add("Attack Range", "How many hexes away a unit can attack from.");
        tooltips.Add("Move", "Press to show valid movement tiles, select one of the valid tiles to trigger movement.");
        tooltips.Add("Attack Value", "Amount of damage dealt when this unit attacks.");
        tooltips.Add("Attack", "Press to show valid attack targets, select one of them to deal your ATK to it.");
        tooltips.Add("Hold Action", "Moves current unit to the end of the turn order, but they keep their action and/or movement if they haven't used it yet.");
        tooltips.Add("End Turn", "Pass your turn to the next unit in the turn order.");

        //right oriented
        tooltips.Add("Abilities", "Click here to see which abilities a unit has!");
        tooltips.Add("Harvest", "Press to show valid harvest targets, select one of the valid targets to Harvest. Harvesting restores all AMR & CRG to the active unit");
        tooltips.Add("Intercept", "COST: 15 CRG | Big Pal moves in a straight line, stopping only when it hits an ally or obstructing terrain. Hitting an ally recovers 10 CRG. Enemies hit along the way take 10 damage and cannot attack on their next turn.");
        tooltips.Add("The Best Defense", "COST: 40 AMR | All adjacent enemies take Big Pal's ATK damage. All adjacent allies recover 10 AMR");
        tooltips.Add("Teardown", "COST: 40 CRG | Attack an adjacent enemy directly to its HP, ignoring its AMR. If the enemy is destroyed, Scrapper can choose to instantly Harvest or use Here, Catch!");
        tooltips.Add("Here, Catch!", "COST: 10 CRG | Harvest an adjacent ruined machine and give the harvest benefits to an ally within 3 hexes in line of sight.");
        tooltips.Add("Mesmerize", "COST: 20 CRG | Target a unit in line of sight. Move that unit and their next movement gets skipped.");
        tooltips.Add("Corpsecall", "COST: 10 CRG | Target a ruined machine in line of sight to move towards the Witch. The first ally it comes in contact with Harvests it. Any enemies hit along the way take the Witch's ATK damage.");
        tooltips.Add("Sacrifice", "COST: BIG PAL | Big Pal's HP is reduced to 0. Target an ally to be restored to full HP, AMR, & CRG. That ally gains 20 ATK.");
        tooltips.Add("Necromancy", "COST: WITCH | Witch's HP is reduced to 0. All dead allies that haven't been harvested are restored to 1 HP & full CRG. Any units revived this way immediately take a turn.");
        tooltips.Add("Last Harvest", "COST: SCRAPPER | Target an adjacent enemy. Scrapper's HP and the enemy's HP are reduced to 0. The enemy creates 3 extra ruined machines adjacent to itself.");
    }

    public void OnPointerEnter(PointerEventData eventData) 
    {
        tooltipBox.SetActive(true);
        tooltipBox.GetComponentInChildren<Text>().text = tag + " | " + tooltips[tag];

        //orientation 1
        if (CompareTag("Health") || CompareTag("Armor") || CompareTag("Charge") || CompareTag("Speed"))
        {
            triangle.localPosition = new Vector3(-350f, 32f, 0f);
            tooltipBox.transform.SetParent(transform);
            tooltipBox.transform.localPosition = Vector3.zero;
            //adjust position
            tooltipBox.transform.localPosition += new Vector3(100f, 40f, 0);
            tooltipBox.transform.SetParent(anchor.transform);
        }

        else if (CompareTag("Threads") || CompareTag("Attack Value") || CompareTag("Attack Range"))
        {
            triangle.localPosition = new Vector3(0f, 32f, 0f);
            tooltipBox.transform.SetParent(transform);
            tooltipBox.transform.localPosition = Vector3.zero;
            //adjust position
            tooltipBox.transform.localPosition += new Vector3(-225f, 40f, 0);
            tooltipBox.transform.SetParent(anchor.transform);
        }
        else if (CompareTag("Move") || CompareTag("Attack") || CompareTag("Hold Action") || CompareTag("End Turn"))
        {
            triangle.localPosition = new Vector3(0f, 32f, 0f);
            tooltipBox.transform.SetParent(transform);
            tooltipBox.transform.localPosition = Vector3.zero;
            //adjust position
            tooltipBox.transform.localPosition += new Vector3(0f, 60f, 0);
            tooltipBox.transform.SetParent(anchor.transform);            
        }
        else
        {
            tooltipBox.transform.SetParent(transform);
            tooltipBox.transform.localPosition = Vector3.zero;
            //adjust position
            tooltipBox.transform.localPosition += new Vector3(-225f, 60f, 0);
            tooltipBox.transform.SetParent(anchor.transform);
            triangle.localPosition = new Vector3(225f, 32f, 0f); 
        }     
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipBox.SetActive(false);
    }
}
