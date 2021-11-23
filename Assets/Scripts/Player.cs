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
            if (ValidMove(gm.selectedTile))
                transform.position = hit.transform.position;
        }
    }

    bool ValidMove(GameObject destinationTile) {
        //list of valid moves
        List<Tuple<int, int>> validCoords = new List<Tuple<int, int>>();

        //lowered position
        if (PlayerUnit.column % 2 != 0) 
        {
            Debug.Log("Lowered");
            validCoords.Add(new Tuple<int, int>( (int)PlayerUnit.column, (int) PlayerUnit.row - 1)); //below
            validCoords.Add(new Tuple<int, int>( (int)PlayerUnit.column, (int) PlayerUnit.row + 1)); //above
            validCoords.Add(new Tuple<int, int>( (int)PlayerUnit.column + 1, (int) PlayerUnit.row)); //upper right
            validCoords.Add(new Tuple<int, int>( (int)PlayerUnit.column + 1, (int) PlayerUnit.row - 1)); //lower right
            validCoords.Add(new Tuple<int, int>( (int)PlayerUnit.column - 1, (int) PlayerUnit.row)); //upper left
            validCoords.Add(new Tuple<int, int>( (int)PlayerUnit.column - 1, (int) PlayerUnit.row - 1)); //lower left 
        } else { //raised position
            Debug.Log("Raised");
            validCoords.Add(new Tuple<int, int>( (int)PlayerUnit.column, (int) PlayerUnit.row - 1)); //below
            validCoords.Add(new Tuple<int, int>( (int)PlayerUnit.column, (int) PlayerUnit.row + 1)); //above
            validCoords.Add(new Tuple<int, int>( (int)PlayerUnit.column + 1, (int) PlayerUnit.row - 1)); //upper right
            validCoords.Add(new Tuple<int, int>( (int)PlayerUnit.column + 1, (int) PlayerUnit.row)); //lower right
            validCoords.Add(new Tuple<int, int>( (int)PlayerUnit.column - 1, (int) PlayerUnit.row - 1)); //upper left
            validCoords.Add(new Tuple<int, int>( (int)PlayerUnit.column - 1, (int) PlayerUnit.row)); //lower left
        }
        
        Tuple<int, int> destCoords = new Tuple<int, int>(destinationTile.GetComponent<Coordinates>().x, destinationTile.GetComponent<Coordinates>().y);
        foreach (var tup in validCoords) {
            Debug.Log(tup);
            if (destCoords == tup) 
            {
                Debug.Log("Does Equal: " + destCoords);
                return true;
            } else {
                Debug.Log("Doesn't Equal: " + destCoords);
            }
        }

        return false;
    }

    //calculate a single step towards the destination
    void CalculateStep() 
    {

    }
}
