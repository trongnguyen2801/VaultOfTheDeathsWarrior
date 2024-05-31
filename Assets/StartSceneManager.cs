using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSceneManager : MonoBehaviour
{
    public GameObject male;
    public GameObject female;
    
    public GameObject cardMale;
    public GameObject cardFemale;

    public void SetActivePlayer(bool sexChoice)
    {
        if (sexChoice)
        {
            male.SetActive(true);
            cardMale.SetActive(false);
            cardFemale.SetActive(false);
        }
        else
        {
            female.SetActive(true);
            cardMale.SetActive(false);
            cardFemale.SetActive(false);
        }
    }

    public void ChoiceAvatar()
    {
        cardMale.SetActive(true);
        cardFemale.SetActive(true);
        male.SetActive(false);
        female.SetActive(false);
    }
}
