using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackController : MonoBehaviour
{
    public GameObject PbAttack;
    private Character _cc;

    private void Awake()
    {
        _cc = GetComponent<Character>();
    }

    public void AttackMagicAoe()
    {
        Instantiate(PbAttack, _cc.targetPlayer.position, Quaternion.identity);
    }

    private void Update()
    {
        _cc.RotateToTarget();
    }
}
