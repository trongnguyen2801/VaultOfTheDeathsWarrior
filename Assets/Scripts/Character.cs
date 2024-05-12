using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour
{
    private CharacterController _cc;
    private Character_Input _characterInput;
    private Animator _animator;
    
    private float _moveSpeed = 1f;
    private Vector3 _movementVelocity;
    private float _verticalVelocity;

    private float _gravity = -0.9f;
    public bool isPlayer = true;
    private float _attackAnimationDuration;
    
    //PlayerSlide
    public float attackStartTime;
    public float attackSlideDuraton = 0.4f;
    public float attackSlideSpeed = 0.1f;
    
    //Combo Melee
    public float attackCoolDown = 1f;
    
    
    //Enemy
    private UnityEngine.AI.NavMeshAgent _navMeshAgent;
    private Transform TargetPlayer;
    
    //DamageCaster
    private DamageCaster _damageCaster;
    
    //Health
    private Health _health;
    
    public enum CharacterState
    {
        Normal, Attacking, Dead, BeingHit, Slide,Spawn,Sprint,Roll
    }
    public CharacterState CurrentState;

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _damageCaster = GetComponentInChildren<DamageCaster>();
        _health = GetComponent<Health>();
        
        if (!isPlayer)
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            TargetPlayer = GameObject.FindWithTag("Player").transform;
            _navMeshAgent.speed = _moveSpeed;
            // SwitchStateTo(CharacterState.Spawn);
            SwitchStateTo(CharacterState.Normal);
        }
        else
        {
            _characterInput = GetComponent<Character_Input>();
        }
    }

    private void CalculateMovementPlayer()
    {
        if (_characterInput.mouseButtonDown && _cc.isGrounded)
        {
            SwitchStateTo(CharacterState.Attacking);
            return;
        }
        if (_characterInput.spaceKeyDown && _cc.isGrounded)
        {
            SwitchStateTo(CharacterState.Roll);
            return;
        }
        
        _movementVelocity.Set(_characterInput.horizontalInput, 0f, _characterInput.verticalInput);
        _movementVelocity.Normalize();
        _movementVelocity = Quaternion.Euler(0, -45f, 0) * _movementVelocity;
        _animator.SetFloat("Run",_movementVelocity.magnitude);
        _movementVelocity *= _moveSpeed * Time.deltaTime;
        
        if (_movementVelocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(_movementVelocity);
        }
    }

    private void CalculateMovementEnemy()
    {
        float _distancePTE = Vector3.Distance(TargetPlayer.position, transform.position);
        if (_distancePTE >= _navMeshAgent.stoppingDistance)
        {
            _navMeshAgent.SetDestination(TargetPlayer.position);
            _animator.SetFloat("Run",0.2f);
        }
        else
        {
            _navMeshAgent.SetDestination(transform.position);
            _animator.SetFloat("Run", 0f);
            SwitchStateTo(CharacterState.Attacking);
        }
    }

    private void FixedUpdate()
    {
        switch (CurrentState)
        {
            case CharacterState.Normal:
                if (isPlayer)
                {
                    CalculateMovementPlayer();
                }
                else
                {
                    CalculateMovementEnemy();
                }
                break;
            case CharacterState.Attacking:
                if (isPlayer)
                {
                    if (Time.deltaTime < attackSlideDuraton + attackStartTime)
                    {
                        float timePassed = Time.time - attackStartTime;
                        float lerpTime = timePassed / attackSlideDuraton;
                        _movementVelocity = Vector3.Lerp(transform.forward * attackSlideSpeed, Vector3.zero, lerpTime);
                    }
                    if (_characterInput.mouseButtonDown && _cc.isGrounded)
                    {
                        _attackAnimationDuration = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                        if (_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "Male Attack 3" && _attackAnimationDuration > 0.5f)
                        {
                            _characterInput.mouseButtonDown = false;
                            SwitchStateTo(CharacterState.Attacking);
                            CalculateMovementPlayer();
                        }
                    }
                }
                break;
            case CharacterState.Slide:
                break;
            case CharacterState.Dead:
                break;
        }
        
        if (isPlayer)
        {
            if (!_cc.isGrounded)
            {
                _verticalVelocity = _gravity;
            }
            else
            {
                _verticalVelocity = _gravity * 0.3f;
            }

            _movementVelocity += _verticalVelocity * Vector3.up * Time.deltaTime;
            _cc.Move(_movementVelocity);
            _movementVelocity = Vector3.zero;
        }
    }

    private void SwitchStateTo(CharacterState newState)
    {
        if (isPlayer)
        {
            _characterInput.ClearCache();
        }

        switch (CurrentState)
        {
            case CharacterState.Normal:
                break;
            case CharacterState.Attacking:
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
        }
        
        switch (newState)
        {
            case CharacterState.Normal:
                break;
            case CharacterState.Attacking:
                if (!isPlayer)
                {
                    Quaternion newRotation = Quaternion.LookRotation(TargetPlayer.position - transform.position);
                    transform.rotation = newRotation;
                }
                
                _animator.SetTrigger("Attack");
                attackStartTime = Time.time;
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
                _animator.SetTrigger("Roll");
                break;
        }

        CurrentState = newState;
    }
    
    public void AttackAnimationEnds()
    {
        Debug.Log("Player attack anim call end");
        SwitchStateTo(CharacterState.Normal);
    }

    public void ApplyDamage(int val)
    {
        _health.ApplyDamage(val);
    }

    public void AddHealth(int val)
    {
        _health.AddHealth(val);
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
