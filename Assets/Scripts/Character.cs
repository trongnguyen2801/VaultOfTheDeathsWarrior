using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
    private CharacterController _cc;
    private Character_Input _input;
    private Animator _animator;
    private PlayerInput _playerInput;
    
    [SerializeField]
    private float _moveSpeed;
    private float _runSpeed = 0.3f;
    private float _sprinteSpeed = 0.4f;
    
    private Vector3 _movementVelocity;
    private float _verticalVelocity;

    private float _gravity = -9.81f;
    public bool isPlayer = true;
    private float _attackAnimationDuration;
    
    //PlayerSlide
    public float attackStartTime;
    public float attackSlideDuraton = 0.4f;
    public float attackSlideSpeed = 0.1f;
    
    //Combo Melee
    public float attackCoolDown = 1f;
    
    
    //Enemy
    private NavMeshAgent _navMeshAgent;
    private Transform TargetPlayer;
    
    //DamageCaster
    private DamageCaster _damageCaster;
    
    //Health
    public Health _health;
    
    public enum CharacterState
    {
        Normal, Attacking, Dead, BeingHit, Slide,Spawn,Sprint,Roll, Jump
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
            _input = GetComponent<Character_Input>();
            _playerInput = GetComponent<PlayerInput>();
        }
    }

    public Vector3 testMoveDirection;
    private void CalculateMovementPlayer()
    {
        _moveSpeed = _input.sprint ? _sprinteSpeed : _runSpeed;
        if (_input.move == Vector2.zero)
        {
            _moveSpeed = 0f;
        }
        
        float inputMagnitude = _input.move.magnitude;
        
        Vector3 moveDirections = new Vector3(_input.move.x,0f,_input.move.y);
        moveDirections = Quaternion.Euler(0, -45f, 0) * moveDirections;
        if (moveDirections != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(moveDirections);
        }
        
        _animator.SetFloat("Speed",_moveSpeed);
        _animator.SetFloat("MotionSpeed",inputMagnitude);
        
        if (!_cc.isGrounded)
        {
            _verticalVelocity = _gravity;
        }
        else
        {
            _verticalVelocity = _gravity * 0.3f;
        }
        
        // move the player
        _cc.Move(moveDirections.normalized * (_moveSpeed * Time.deltaTime) +
                 new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
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
            StartCoroutine(WaitForSeconds(0.3f));
            SwitchStateTo(CharacterState.Attacking);
        }
    }

    IEnumerator WaitForSeconds(float sec)
    {
        yield return new WaitForSeconds(sec);
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
                    // if (_characterInput.mouseButtonDown && _cc.isGrounded)
                    // {
                    //     _attackAnimationDuration = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                    //     if (_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "Male Attack 3" && _attackAnimationDuration > 0.5f)
                    //     {
                    //         _characterInput.mouseButtonDown = false;
                    //         SwitchStateTo(CharacterState.Attacking);
                    //         CalculateMovementPlayer();
                    //     }
                    // }
                }
                break;
            case CharacterState.Slide:
                break;
            case CharacterState.Dead:
                break;
        }
    }

    private void SwitchStateTo(CharacterState newState)
    {
        if (isPlayer)
        {
            // _characterInput.ClearCache();
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
            case CharacterState.Jump:
                _animator.SetTrigger("Jump");
                break;
        }

        CurrentState = newState;
    }
    
    public void AttackAnimationEnds()
    {
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
