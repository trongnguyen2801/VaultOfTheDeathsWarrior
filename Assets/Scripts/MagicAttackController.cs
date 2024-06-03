using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicAttackController : MonoBehaviour
{
    public int damage = 30;
    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        Character cc = other.gameObject.GetComponent<Character>();
        if (damageable != null && cc != null && cc.isPlayer)
        {
            damageable.ApplyDamage(damage);
            // _cc.ApplyDamage(damage,transform.position);
        }

        StartCoroutine(DestroyObject(2f));
    }

    IEnumerator DestroyObject(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
