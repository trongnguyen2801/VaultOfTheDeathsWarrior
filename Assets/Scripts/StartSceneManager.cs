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

    public GameObject selectCharacterScreen;
    public GameObject propertyCharacterScreen;

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

        if (PlayerPrefs.GetInt("StartScreen") == 0)
        {
            selectCharacterScreen.SetActive(true);
        }
        else
        {
            propertyCharacterScreen.SetActive(true);
        }
        Debug.Log("screen " + PlayerPrefs.GetInt("StartScreen"));
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
        PlayerPrefs.SetInt("PlayerSex",1);
        // DataManager.Instance.SaveData(DataManager.dataName.PlayerSex,1);
    }
    public void SelectMalePlayer()
    {
        lightFemale.SetActive(false);
        textNameFemale.gameObject.SetActive(false);
        lightMale.SetActive(true);
        textNameMale.gameObject.SetActive(true);
        animMale.SetTrigger("Attack");
        PlayerPrefs.SetInt("PlayerSex",0);
        // DataManager.Instance.SaveData(DataManager.dataName.PlayerSex,0);
    }

    public void SwapScreenStartScene(int screen)
    {
        selectCharacterScreen.SetActive(false);
        propertyCharacterScreen.SetActive(true);
        PlayerPrefs.SetInt("StartScreen",1);
    }
}
