using System;
using System.Collections;
using System.Collections.Generic;
using DissolveExample;
using UnityEngine;

public class StartSceneManager : MonoBehaviour
{
    public Animator animMale;
    public Animator animFemale;

    public DissolveChilds dissolveSwordFemale;
    public DissolveChilds dissolveSwordMale;

    public GameObject lightMale;
    public GameObject lightFemale;
    
    public GameObject textNameMale;
    public GameObject textNamefemale;

    private void Start()
    {
        
    }

    public void SelectFemalePlayer()
    {
        lightFemale.SetActive(true);
        animFemale.SetTrigger("Attack");
        lightMale.SetActive(false);
        
    }
    public void SelectMalePlayer()
    {
        lightFemale.SetActive(false);
        lightMale.SetActive(true);
        animMale.SetTrigger("Attack");
    }
    
}
