using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileManager : MonoBehaviour
{
    public  Text name;
    public  Text level;
    public Image avatar;
    public Image healthBar;
    public Image manaBar;

    private static ProfileManager _instance;
    public static ProfileManager Instance => _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
        }
        
        healthBar.fillAmount = 1f;
        manaBar.fillAmount = 1f;
    }

    public void SetProfile(string _name, string _avatarId, string _level)
    {
        name.text = _name;
        // avatar.sprite = 
        level.text = _level;
    }

    public void SetHealthAndMana(float _health, float _mana)
    {
        healthBar.fillAmount = _health;
        manaBar.fillAmount = _mana;
    }
}
