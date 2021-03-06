using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private PositionManager pm = new PositionManager();
    
    public GameManager gm;
    public GameObject board;

    private LayerMask layerMask;
    
    private bool active;
    public Unit PlayerUnit;

    private bool generatedMovement;

    public List<Tuple<int, int>> validCoords = new List<Tuple<int, int>>();


    //On each turn player can
        //move or not move
        //&
        //attack or use an ability or harvest a fallen enemy or meltdown    

    // Start is called before the first frame update
    void Start()
    {
        layerMask = LayerMask.GetMask("Tiles");
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        board = GameObject.Find("Board");
    }

    // Update is called once per frame
    void Update()
    {
        if (gm.ActiveUnit.unitName == PlayerUnit.unitName) 
        {
            active = true;
        }
        if (!generatedMovement) {
            ValidMove(gm.CurrentBoard[(int)PlayerUnit.column, (int)PlayerUnit.row]);
            AdjustIndicators();
        }
            

        //raycast to select a tile
        RaycastAndSelectTile();
    }

    void RaycastAndSelectTile() {
        Vector3 mPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 5));
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(mPos.x, mPos.y), Vector2.zero, layerMask);
        if (hit && active && Input.GetMouseButtonDown(0)) 
        {
            gm.selectedTile = hit.transform.gameObject;
            Debug.Log("Player's Current Position: (" + PlayerUnit.column + ", " + PlayerUnit.row + ")");
            Debug.Log("Player's Destination: (" + gm.selectedTile.GetComponent<Coordinates>().x + ", " + gm.selectedTile.GetComponent<Coordinates>().y + ")");
            Tuple<int, int> dest = new Tuple<int, int>(gm.selectedTile.GetComponent<Coordinates>().x, gm.selectedTile.GetComponent<Coordinates>().y);
            if (validCoords.Contains(dest)) { //change to if move in validCoords
                transform.position = hit.transform.position;
                PlayerUnit.column = dest.Item1; PlayerUnit.row = dest.Item2;
                generatedMovement = false;
            }
        }
    }

    void ValidMove(GameObject destinationTile) {
        //to ensure no reentering this function
        generatedMovement = true;
        //destination tuple
        Tuple<int, int> destCoords = new Tuple<int, int>(destinationTile.GetComponent<Coordinates>().x, destinationTile.GetComponent<Coordinates>().y);
        //calculate first step from starting spot
        validCoords = CalculateStep(new Tuple<int, int>( (int)PlayerUnit.column, (int) PlayerUnit.row));
        //buffer list
        List<Tuple<int, int>> buf = new List<Tuple<int, int>>();
        //second buffer
        List<Tuple<int, int>> temp = new List<Tuple<int, int>>();       
        //Keep going until either find spot or run out of spd
        for (int i = 1; i < PlayerUnit.SPD; i++) {
            //expand
            foreach (var tup in validCoords) {
                //load possibilties into buffer
                buf = CalculateStep(tup);
                //move them into temp to ensure foreach loop isn't messed up
                foreach (var tup2 in buf)
                    temp.Add(tup2);
            }
            //transfer from temp to validcoords
            foreach (var tup in temp) {
                validCoords.Add(tup);
            }
        }
    }

    //calculate a single step towards the destination
    List<Tuple<int, int>> CalculateStep(Tuple<int, int> origin) 
    {
        List<Tuple<int, int>> temp = new List<Tuple<int, int>>();
        //if its in a lowered column
        if (origin.Item1 % 2 != 0) 
        {
            //lowered
            temp.Add(new Tuple<int, int>( origin.Item1, origin.Item2 - 1)); //below
            temp.Add(new Tuple<int, int>( origin.Item1, origin.Item2 + 1)); //above
            temp.Add(new Tuple<int, int>( origin.Item1 + 1, origin.Item2)); //upper right
            temp.Add(new Tuple<int, int>( origin.Item1 + 1, origin.Item2 - 1)); //lower right
            temp.Add(new Tuple<int, int>( origin.Item1 - 1, origin.Item2)); //upper left
            temp.Add(new Tuple<int, int>( origin.Item1 - 1, origin.Item2 - 1)); //lower left 
        } else {
            //raised
            temp.Add(new Tuple<int, int>( origin.Item1, origin.Item2 - 1)); //below
            temp.Add(new Tuple<int, int>( origin.Item1, origin.Item2 + 1)); //above
            temp.Add(new Tuple<int, int>( origin.Item1 + 1, origin.Item2 + 1)); //upper right
            temp.Add(new Tuple<int, int>( origin.Item1 + 1, origin.Item2)); //lower right
            temp.Add(new Tuple<int, int>( origin.Item1 - 1, origin.Item2 + 1)); //upper left
            temp.Add(new Tuple<int, int>( origin.Item1 - 1, origin.Item2)); //lower left
        }
        return temp;
    }

    void AdjustIndicators() {
        //clear all indicators
        foreach (var tile in gm.CurrentBoard) {
            tile.GetComponent<Coordinates>().Indicator.SetActive(false);
        }
        //generate indicators
        foreach (var coord in validCoords) {
            if ((coord.Item1 > -1 && coord.Item2 < 10) && (coord.Item2 > -1 && coord.Item2 < 10)) {
                GameObject temp = gm.CurrentBoard[coord.Item1, coord.Item2];
                temp.GetComponent<Coordinates>().Indicator.SetActive(true);
            }
        }
    }

}
