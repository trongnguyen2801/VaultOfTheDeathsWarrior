using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicAttackController : MonoBehaviour
{
    public int damage = 30;
    private void OnTriggerEnter(Collider other)
    {
        Character _cc = other.gameObject.GetComponent<Character>();
        if (_cc != null && _cc.isPlayer)
        {
            _cc.ApplyDamage(damage,transform.position);
        }

        StartCoroutine(DestroyObject(2f));
    }

    IEnumerator DestroyObject(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
