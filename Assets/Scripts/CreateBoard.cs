using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CreateBoard : MonoBehaviour {

    public GameObject BlackSquare;
    public GameObject WhiteSquare;
    public int width;
    public int height;
    

    void Start() {
        for(int r = 0; r < width; r++)
        {
            for(int c = 0; c < height; c++)
            {  
                if ((r+c) % 2 == 0)
                { 
                    var myObj = Instantiate(BlackSquare, new Vector3(r, c, 0), Quaternion.identity);
                    myObj.transform.SetParent(transform);
                } 
                else
                { 
                    var myObj = Instantiate(WhiteSquare, new Vector3(r, c, 0), Quaternion.identity);
                    myObj.transform.SetParent(transform);
                }            
            }          
        }
    }
}
