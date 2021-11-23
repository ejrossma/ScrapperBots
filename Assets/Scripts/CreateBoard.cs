using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CreateBoard : MonoBehaviour {

    public GameObject DarkHex;
    public GameObject LightHex;
    public int width;
    public int height;
    
    public GameManager gm;

    void Start() {
        for(int r = 0; r < width; r++)
        {
            for(int c = 0; c < height; c++)
            {  
                var cModifier = r % 2 == 1 ? -0.5f : 0f;
                GameObject HexColor;
                //one instantiate but change color
                if (r % 2 == 0) 
                {
                    HexColor = (r+c) % 2 == 0 ? DarkHex : LightHex;
                } else {
                    HexColor = (r+c) % 2 == 0 ? LightHex : DarkHex;
                }
                var myObj = Instantiate(HexColor, new Vector3(r * 0.87f, c + cModifier, 0), Quaternion.identity);
                myObj.transform.SetParent(transform);
                myObj.layer = LayerMask.NameToLayer("Tiles");
                myObj.transform.gameObject.GetComponent<Coordinates>().x = r;
                myObj.transform.gameObject.GetComponent<Coordinates>().y = c;
                gm.CurrentBoard[r,c] = myObj;
            }
        }
    }
}
