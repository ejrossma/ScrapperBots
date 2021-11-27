using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpendScrapControl : MonoBehaviour
{
    public GameObject ScrapPanel;
    public GameObject SkillsCol1Panel;
   
    public void OpenScrapPanel()
    {
        ScrapPanel.SetActive(true);
        SkillsCol1Panel.SetActive(false);
    }

    public void CloseScrapPanel()
    {
        ScrapPanel.SetActive(false);
        SkillsCol1Panel.SetActive(true);
    }
}
