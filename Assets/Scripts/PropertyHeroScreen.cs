using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PropertyHeroScreen : MonoBehaviour
{
    public GameObject maleCharacter;
    public GameObject femaleCharacter;

    void Start()
    {
        Debug.Log("Data manager + " + DataManager.Instance.LoadDataInt(DataManager.dataName.PlayerSex));
        Debug.Log("player pref sex + " + PlayerPrefs.GetInt("PlayerSex"));

        if (PlayerPrefs.GetInt("PlayerSex") == 1)
        {
            femaleCharacter.SetActive(true);
            Debug.Log("dcm may game lon");
        }
        else
        {
            maleCharacter.SetActive(true);
            Debug.Log("Game lon  + " + PlayerPrefs.GetInt("PlayerSex"));
        }
    }

    public void StartGameScene()
    {
        SceneManager.LoadScene("MainScene");
    }
}
