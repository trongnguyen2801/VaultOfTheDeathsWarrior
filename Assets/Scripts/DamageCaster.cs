using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCaster : MonoBehaviour
{ 
    private Collider _damageCaster;
    public string targetTag;
    private Character _targetCC;
    private List<Collider> _damageTargetList;
    public int damage = 30;
    private void Awake()
    {
        _damageCaster = GetComponentInChildren<Collider>();
        _damageTargetList = new List<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag) && _damageTargetList.Contains(other))
        {
            _targetCC = other.GetComponent<Character>();
            _targetCC.ApplyDamage(damage);
        }
    }

    public void EnableDamageCaster()
    {
        _damageCaster.enabled = true;
    }

    public void DisableDamageCaster()
    {
        _damageCaster.enabled = false;
    }
}
