using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class Character : MonoBehaviour
{
    private CharacterController _cc;
    private Character_Input _input;
    private Animator _animator;
    private PlayerInput _playerInput;
    
    [SerializeField]
    public float _moveSpeed;
    public float _runSpeed = 0.3f;
    public float _sprintSpeed = 0.4f;
    public float JumpHeight = 2.2f;
    public bool isPlayer = true;

    public InputActionReference attack;
    
    [Space(10)]
    private float _attackAnimationDuration;
    private float _gravity = -9.81f;
    private Vector3 _verticalVelocity;
    private float _fallTimeoutDelta;
    private float _jumpTimeoutDelta;

    [Space(10)]
    public float JumpTimeout = 0.3f;
    public float FallTimeout = 0.15f;
    
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

    private void Start()
    {
        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;
    }

    private void CalculateMovementPlayer()
    {
        if (_input.attack && _cc.isGrounded)
        {
            SwitchStateTo(CharacterState.Attacking);
            return;
        }
        
        _moveSpeed = _input.sprint ? _sprintSpeed : _runSpeed;
        
        if (_input.move == Vector2.zero) _moveSpeed = 0.0f;
        if (_cc.isGrounded && _verticalVelocity.y < 0) _verticalVelocity.y = 0;
        
        float inputMagnitude = _input.move.magnitude;
        
        Vector3 moveDirections = new Vector3(_input.move.x,0f,_input.move.y);
        moveDirections = Quaternion.Euler(0, -45f, 0) * moveDirections;
        
        if (moveDirections != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(moveDirections);
        }

        _animator.SetFloat("Speed",_moveSpeed);
        _animator.SetFloat("MotionSpeed",inputMagnitude);
        
        // move the player
        _cc.Move(moveDirections * (_moveSpeed * Time.deltaTime) +
        new Vector3(0.0f, _verticalVelocity.y, 0.0f) * Time.deltaTime);
    }
    
    private void JumpAndGravity()
    {
        if (_cc.isGrounded)
        {
            _fallTimeoutDelta = FallTimeout;
                        
            //stop dropping infinite when grounded
            if (_verticalVelocity.y < 0.0f)
            {
                _verticalVelocity.y = -2f;
            }

            if (_input.jump && _jumpTimeoutDelta <= 0.0f)
            {
                _verticalVelocity.y += Mathf.Sqrt(JumpHeight * -3.0f * _gravity);
            }
            
            // if (_hasAnimator)
            // {
            //     _animator.SetBool(_animIDJump, false);
            //     _animator.SetBool(_animIDFreeFall, false);
            // }
            
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            _jumpTimeoutDelta = JumpTimeout;
            
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            
            _verticalVelocity.y += _gravity * Time.deltaTime;
            _input.jump = false;
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
                    JumpAndGravity();
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
                    // if (Time.deltaTime < attackSlideDuraton + attackStartTime)
                    // {
                    //     float timePassed = Time.time - attackStartTime;
                    //     float lerpTime = timePassed / attackSlideDuraton;
                    //     _input.move = Vector3.Lerp(transform.forward * attackSlideSpeed, Vector3.zero, lerpTime);
                    // }
                    if (_input.attack && _cc.isGrounded)
                    {
                        _attackAnimationDuration = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                        if (_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "Male Attack 3" && _attackAnimationDuration > 0.7f)
                        {
                            _input.attack = false;
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
    }

    private void SwitchStateTo(CharacterState newState)
    {
        if (isPlayer)
        {
            _input.ClearCache();
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
        Debug.Log("Call anim end");
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
