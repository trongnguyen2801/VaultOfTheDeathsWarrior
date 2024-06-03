using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackMagic : MonoBehaviour
{
    public GameObject PbAttack;
    private Character _cc;
    private Enemy _enemy;

    private void Awake()
    {
        _cc = GetComponent<Character>();
        _enemy = GetComponent<Enemy>();
    }

    public void AttackMagicAoe()
    {
        Instantiate(PbAttack, _enemy.targetPlayer.position, Quaternion.identity);
    }

    private void Update()
    {
        _enemy.RotateToTarget();
    }
}
