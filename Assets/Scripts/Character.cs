using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class Character : MonoBehaviour
{
    private Player _player;
    private Enemy _enemy;
    private Animator _animator;

    public bool isPlayer = true;
    
    //DamageCaster
    private DamageCaster _damageCaster;

    #region Finite State Machine Variables

    public enum CharacterState
    {
        Normal, Attacking, Dead, BeingHit, Slide, Spawn, Sprint, Roll, Jump
    }
    public CharacterState CurrentState;

    #endregion


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _player = GetComponent<Player>();
        _enemy = GetComponent<Enemy>();
    }

    private void Start()
    {
        _damageCaster = GetComponentInChildren<DamageCaster>();
    }

    private void FixedUpdate()
    {
        switch (CurrentState)
        {
            case CharacterState.Normal:
                if (isPlayer)
                {
                    _player.JumpAndGravity();
                    _player.CalculateMovementPlayer();
                }
                else
                {
                    _enemy.CalculateMovementEnemy();
                }
                break;
            case CharacterState.Attacking:
                if (isPlayer)
                {
                   _player.SlidePlayerAttack();
                   _player.PlayerAttackCombo();
                }
                break;
            case CharacterState.Slide:
                break;
            case CharacterState.BeingHit:
                _player.PlayerBeingHit();
                break;
            case CharacterState.Dead:
                break;
        }

        if (isPlayer)
        {
            _player.PlayerMove();
        }
    }

    public void SwitchStateTo(CharacterState newState)
    {
        if (isPlayer)
        {
            _player.input.ClearCache();
        }
        switch (CurrentState)
        {
            case CharacterState.Normal:
                break;
            case CharacterState.Attacking:
                if (_damageCaster != null)
                {
                    DisableDamageCaster();
                }

                if (isPlayer)
                {
                    _player.StopBladeAnimation();
                }
                break;
            case CharacterState.Sprint:
                break;
            case CharacterState.Slide:
                break;
            case CharacterState.Spawn:
                break;
            case CharacterState.Dead:
                break;
            case CharacterState.Roll:
                break;
            case CharacterState.Jump:
                break;
        }
        
        switch (newState)
        {
            case CharacterState.Normal:
                break;
            case CharacterState.Attacking:
                if (!isPlayer)
                {
                    _enemy.LookAtTarget();
                }
                
                
                _animator.SetTrigger(GameManager.Instance.animIDAttack);
                if (isPlayer)
                {
                    _player.attackStartTime = Time.time;
                }
                break;
            case CharacterState.Sprint:
                break;
            case CharacterState.Slide:
                break;
            case CharacterState.BeingHit:
                _animator.SetTrigger(GameManager.Instance.animIDBeingHit);
                if (isPlayer)
                {
                    _player.InviciblePlayer();
                }
                break;
            case CharacterState.Spawn:
                break;
            case CharacterState.Dead:
                _player.Die();
                _animator.SetTrigger(GameManager.Instance.animIDDead);
                break;
            case CharacterState.Roll:
                _animator.SetTrigger(GameManager.Instance.animIDRoll);
                break;
            case CharacterState.Jump:
                break;
        }

        CurrentState = newState;
    }
    
    public void AttackAnimationEnds()
    {
        SwitchStateTo(CharacterState.Normal);
    }

    public void BeingHitAnimationEnds()
    {
        SwitchStateTo(CharacterState.Normal);
    }
    
    public void RollAnimationEnds()
    {
        SwitchStateTo(CharacterState.Normal);
    }

    public void EnableDamageCaster()
    {
        _damageCaster.EnableDamageCaster();
    }
    
    public void DisableDamageCaster()
    {
        _damageCaster.DisableDamageCaster();
    }
}
