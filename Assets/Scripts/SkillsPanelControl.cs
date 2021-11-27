using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillsPanelControl : MonoBehaviour
{
    public GameObject SkillsPanel;
    public GameObject SkillsCol2Panel;

    public void OpenSkillsPanel()
    {
        SkillsPanel.SetActive(true);
        SkillsCol2Panel.SetActive(false);
    }

    public void CloseSkillsPanel()
    {
        SkillsPanel.SetActive(false);
        SkillsCol2Panel.SetActive(true);
    }
}
