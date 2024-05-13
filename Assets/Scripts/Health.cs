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

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public void ApplyDamage(int damage)
    {
        if (_currentHealth > 0)
        {
            _currentHealth -= damage;
        }

        if (_currentHealth <= 0)
        {
            Debug.Log("death");
        }
        Debug.Log(_currentHealth + " " + gameObject.name);
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
