using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamageable
{
    //Enemy
    private NavMeshAgent _navMeshAgent;
    private Transform _targetPlayer;
    public Transform targetPlayer => _targetPlayer;
    private Animator _animator;
    private Character _cc;
    
    public float MaxHealth { get; set; }
    public float CurrentHealth { get; set; }
    private void Awake()
    {
        _cc = GetComponent<Character>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _targetPlayer = GameObject.FindWithTag("Player").transform;
        _navMeshAgent.speed = 2f;
        _cc.SwitchStateTo(Character.CharacterState.Normal);
    }

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        MaxHealth = 100f;
        CurrentHealth = MaxHealth;
    }

    
    public void CalculateMovementEnemy()
    {
        if (Vector3.Distance(_targetPlayer.position, transform.position) >= _navMeshAgent.stoppingDistance)
        {
            _navMeshAgent.SetDestination(_targetPlayer.position);
            _animator.SetFloat(GameManager.Instance.animIDWalk,_navMeshAgent.speed);
            // Debug.Log(Vector3.Distance(_targetPlayer.position, transform.position));
        }
        else
        {
            _navMeshAgent.SetDestination(transform.position);
            _animator.SetFloat(GameManager.Instance.animIDWalk, 0f);
            StartCoroutine(WaitForSeconds(0.3f));
            _cc.SwitchStateTo(Character.CharacterState.Attacking);
        }
    }

    IEnumerator WaitForSeconds(float sec)
    {
        yield return new WaitForSeconds(sec);
    }
    
    public void ApplyDamage(float dmg, Vector3 posAttack = new Vector3())
    {
        CurrentHealth -= dmg;
        Debug.Log("enemy apply damage" + CurrentHealth);
    }
    
    public void RotateToTarget()
    {
        if (_cc.CurrentState != Character.CharacterState.Dead)
        {
            transform.LookAt(_targetPlayer, Vector3.up);
        }
    }

    public void LookAtTarget()
    {
        Quaternion newRotation = Quaternion.LookRotation(_targetPlayer.position - transform.position);
        transform.rotation = newRotation;
    }

    public void SetAnimatorClip(int _anima)
    {
        _animator.SetTrigger(_anima);
    }

    public void Die()
    {
        
    }
}
