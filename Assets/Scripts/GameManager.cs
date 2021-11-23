using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //Array holding the current turn order
        //inside the array is a reference to the unit so it can be found on the board

    //Combat is handled based on speed/initiative
        //After each character takes their turn it goes to the next round of combat

    private PositionManager pm = new PositionManager();
    
    public Unit ActiveUnit;
    public GameObject BattleCanvas;
    public GameObject Player;

    //Two dimensional array that holds references to the tiles
        //Board Positions can be referenced from 0-9 (0,0 is bottom left & 9,9 is top right)
    public GameObject[,] CurrentBoard = new GameObject[10,10];

    void Start()
    {
        //Place the player in the bottom left and store them in the currentboard
        var tempPos = Player.GetComponent<Player>().PlayerUnit.row;
        var unit = Instantiate(Player, new Vector3(Player.GetComponent<Player>().PlayerUnit.column * pm.moveModifier, 
                                                   tempPos - 0.5f, -1), Quaternion.identity);
        ActiveUnit = Player.GetComponent<Player>().PlayerUnit;
    }

    //This is the highest level of abstraction in the project, use functions and other files to handle all tasks
    void Update()
    {
        updateStats();
    }









    //CHANGE SO THAT IT UPDATES TO THE CURRENT UNIT AND THAT THE ENEMY ONE UPDATES BASED
    //ON WHO THE PLAYER HAS CLICKED ON
    void updateStats() {
        var PlayerTeam = BattleCanvas.transform.GetChild(0);

        var classString = "Class: " + Player.GetComponent<Player>().PlayerUnit.unitClass;
        var temp = PlayerTeam.GetChild(1).GetComponent<Text>().text = classString;
        temp = PlayerTeam.GetChild(2).GetComponent<Text>().text = "Level: " + Player.GetComponent<Player>().PlayerUnit.LVL;
        temp = PlayerTeam.GetChild(3).GetComponent<Text>().text = "Health: " + Player.GetComponent<Player>().PlayerUnit.HP;
        temp = PlayerTeam.GetChild(4).GetComponent<Text>().text = "Armor: " + Player.GetComponent<Player>().PlayerUnit.AMR;
        temp = PlayerTeam.GetChild(5).GetComponent<Text>().text = "Attack: " + Player.GetComponent<Player>().PlayerUnit.ATK;
        temp = PlayerTeam.GetChild(6).GetComponent<Text>().text = "Charge: " + Player.GetComponent<Player>().PlayerUnit.CRG;
        temp = PlayerTeam.GetChild(7).GetComponent<Text>().text = "Speed: " + Player.GetComponent<Player>().PlayerUnit.SPD;
        temp = PlayerTeam.GetChild(8).GetComponent<Text>().text = "Threads: " + Player.GetComponent<Player>().PlayerUnit.TRD;
    }
}
