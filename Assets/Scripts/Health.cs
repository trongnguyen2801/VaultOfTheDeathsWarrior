using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField]
    private int _maxHealth = 100;
    [SerializeField]
    private int _currentHealth;
    private Character _cc;

    private void Awake()
    {
        _currentHealth = _maxHealth;
        _cc = GetComponent<Character>();
    }

    public void ApplyDamage(int damage)
    {
        if (_currentHealth > 0)
        {
            _currentHealth -= damage;
        }
        else
        {
            Debug.Log("death");
            _cc.SwitchStateTo(Character.CharacterState.Dead);
        }

        if (_cc.isPlayer)
        {
            float perHealth = _currentHealth / 100f;
            ProfileManager.Instance.SetHealthAndMana(perHealth,1f);
            Debug.Log(perHealth + " " + gameObject.name);
        }
    }

    public void AddHealth(int val)
    {
        if (_currentHealth + val <= _maxHealth)
        {
            _currentHealth += val;
        }
        else
        {
            _currentHealth = _maxHealth;
        }
    }
}
