using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SpendXScrapScript : MonoBehaviour
{
    public Slider slider;
    private Text scrapValue;

    void Start()
    {
        scrapValue = GetComponent<Text>();
        ShowScrapValue();
    }

    public void ShowScrapValue()
    {
        string printedText = "Spend " + slider.value + " scrap?";
        scrapValue.text = printedText;
    }
}
