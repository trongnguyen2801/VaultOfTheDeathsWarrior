using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NavbarBehavior : MonoBehaviour
{
    public TextMeshProUGUI[] textNavbar;
    public GameObject[] listTab;
    
    public void ClickToButton(TextMeshProUGUI textClick)
    {
        foreach (var t in textNavbar)
        {
            t.color = new Color(140f,140f,140f,255f);
            t.fontSize = 36f;
        }
        textClick.color = Color.white;
        textClick.fontSize = 45f;
        Debug.Log(textClick.name);
    }

    public void InventoryTabShow()
    {
        foreach (var t in listTab)
        {
            t.SetActive(false);
        }
    }
    
    public void SkillsTabShow()
    {
        
    }
    
    public void WeaponsTabShow()
    {
        
    }
}
