using System;
using System.Collections;
using System.Collections.Generic;
using DissolveExample;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartSceneManager : MonoBehaviour
{
    public Animator animMale;
    public Animator animFemale;

    public DissolveChilds dissolveSwordFemale;
    public DissolveChilds dissolveSwordMale;

    public GameObject lightMale;
    public GameObject lightFemale;
    
    public TextMeshProUGUI textNameMale;
    public TextMeshProUGUI textNameFemale;
    
    List<Material> materials = new List<Material>();

    private void Start()
    {
        var renders = GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renders.Length; i++)
        {
            materials.AddRange(renders[i].materials);
        }

        SetValue(0);
    }
    
    public void SetValue(float value)
    {
        for (int i = 0; i < materials.Count; i++)
        {
            materials[i].SetFloat("_Dissolve", value);
        }
    }

    public void SelectFemalePlayer()
    {
        lightFemale.SetActive(true);
        textNameFemale.gameObject.SetActive(true);
        animFemale.SetTrigger("Attack");
        lightMale.SetActive(false);
        textNameMale.gameObject.SetActive(false);
    }
    public void SelectMalePlayer()
    {
        lightFemale.SetActive(false);
        textNameFemale.gameObject.SetActive(false);
        lightMale.SetActive(true);
        textNameMale.gameObject.SetActive(true);
        animMale.SetTrigger("Attack");
    }
}
