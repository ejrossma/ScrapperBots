using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideTeamOverviewScript : MonoBehaviour
{
    //for debugging
    //int n;

    public void OnButtonPress()
    {
        //for debugging
        //n++;
        //Debug.Log("Button clicked " + n + " times.");

        //get the desired game object
        var panelGroup = GameObject.Find("TeamOverviewSidebar");
        var hideButton = GameObject.Find("HideTeamOverview");
        //var buttonYPos = hideButton.transform.position.y;
        //access the canvas group
        var getCanvasGroup = panelGroup.GetComponent<CanvasGroup>();

        //if the canvass group is hidden, reveal it and make it interactable
        
        if (getCanvasGroup.alpha == 0)
        {
            getCanvasGroup.alpha = 1;
            getCanvasGroup.interactable = true;
            //hideButton.transform.Translate(0f, 210f, 0f);

        }

        //otherwise hide it, and set interactable to false
        else
        {
            getCanvasGroup.alpha = 0;
            getCanvasGroup.interactable = false;
            //hideButton.transform.Translate(0f, -210f, 0f);
        }
 
    }
}
