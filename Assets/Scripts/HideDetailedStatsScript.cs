using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideDetailedStatsScript : MonoBehaviour
{
    //for debugging
    //int n;

    public void OnButtonPress()
    {
        //for debugging
        //n++;
        //Debug.Log("Button clicked " + n + " times.");

        //get the desired game object
        var panelGroup = GameObject.Find("PopoutDetailedStatsandSkills");
        //access the canvas group
        var getCanvasGroup = panelGroup.GetComponent<CanvasGroup>();

        //if the canvass group is hidden, reveal it and make it interactable
        /*
        if (getCanvasGroup.alpha == 0)
        {
            getCanvasGroup.alpha = 1;
            getCanvasGroup.interactable = true;

        }

        //otherwise hide it, and set interactable to false
        else
        {
            getCanvasGroup.alpha = 0;
            getCanvasGroup.interactable = false;
        }
        */

        //get the position
        var ypos = getCanvasGroup.transform.position.y;

        var viewable = 0f;
        var hidden = 0f;

        //if the panel is in the viewable position, move it off the screen
        if (ypos > viewable)
        {
            getCanvasGroup.transform.Translate(0f, -300f, 0f);
        }
        //else move it onto the screen
        else if (ypos < hidden)
        {
            getCanvasGroup.transform.Translate(0f, 300f, 0f);
        }

    }
}
