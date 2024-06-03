using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class CharacterFirstDesign : MonoBehaviour
{
    private CharacterController _cc;
    private Character_Input _input;
    private Animator _animator;
    
    public float _moveSpeed;
    public float _runSpeed = 2.5f;
    public float _sprintSpeed = 4f;
    public float JumpHeight = 2.2f;
    public bool isPlayer = true;

    public Avatar maleAvatar;
    public Avatar feMaleAvatar;

    
    [Space(10)]
    //Visual player
    public GameObject visualMale;
    public GameObject visualFemale;
    
    //Set sex player
    // true is male, false is female
    public bool isMale;

    
    [Space(10)]
    private float _attackAnimationDuration;
    private float _gravity = -9.81f;
    
    private Vector3 _verticalVelocity;
    private Vector3 _movementVelocity;
    private Vector3 _impactOnPlayer;
    
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
    
    // animation IDs
    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFall;
    private int _animIDMotionSpeed;
    private int _animIDWalk;
    private int _animIDDead;
    private int _animIDBeingHit;
    private int _animIDAttack;
    private int _animIDRoll;
    private int _canAttack;

    //Enemy
    private NavMeshAgent _navMeshAgent;
    private Transform _targetPlayer;
    public Transform targetPlayer => _targetPlayer;
    
    //DamageCaster
    private DamageCaster _damageCaster;

    private bool _isInvincible;
    private float _invincibleDuration = 1f;
    public enum CharacterState
    {
        Normal, Attacking, Dead, BeingHit, Slide, Spawn, Sprint, Roll, Jump
    }
    public CharacterState CurrentState;
    
    private bool _hasAnimator;
    public float attackComboCount;

    private void Awake()
    {
        if (!isPlayer)
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _targetPlayer = GameObject.FindWithTag("Player").transform;
            _navMeshAgent.speed = 2f;
            // SwitchStateTo(CharacterState.Spawn);
            SwitchStateTo(CharacterState.Normal);
        }
        else
        {
            _input = GetComponent<Character_Input>();
        }
    }

    private void Start()
    {
        _animator = GetComponent<Animator>();
        if (isPlayer)
        {
            if (isMale)
            {
                visualMale.SetActive(true);
                _animator.avatar = maleAvatar;
            }
            else
            {
                visualFemale.SetActive(true);
                _animator.avatar = feMaleAvatar;
            }
        }
        _cc = GetComponent<CharacterController>();
        _damageCaster = GetComponentInChildren<DamageCaster>();
        _hasAnimator = TryGetComponent(out _animator);
        
        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;
        _isInvincible = false; // tesst
        AssignAnimationIDs();
        attackComboCount = 1;
    }
    
    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Ground");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFall = Animator.StringToHash("Fall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        _animIDWalk = Animator.StringToHash("Walk");
        _animIDDead = Animator.StringToHash("Dead");
        _animIDBeingHit = Animator.StringToHash("BeingHit");
        _animIDAttack = Animator.StringToHash("Attack");
        _animIDRoll = Animator.StringToHash("Roll");
    }

    private void CalculateMovementPlayer()
    {
        if (_input.attack && _cc.isGrounded && attackComboCount < 5)
        {
            SwitchStateTo(CharacterState.Attacking);
            return;
        }
        
        if (_input.roll && _cc.isGrounded)
        {
            SwitchStateTo(CharacterState.Roll);
            return;
        }
        
        _moveSpeed = _input.sprint ? _sprintSpeed : _runSpeed;
        
        if (_input.move == Vector2.zero) _moveSpeed = 0.0f;
        if (_cc.isGrounded && _verticalVelocity.y < 0) _verticalVelocity.y = 0;
        
        float inputMagnitude = _input.move.magnitude;
        
        _movementVelocity = new Vector3(_input.move.x,0f,_input.move.y);
        _movementVelocity.Normalize();
        _movementVelocity = Quaternion.Euler(0, -45f, 0) * _movementVelocity;
        _movementVelocity *= _moveSpeed * Time.deltaTime;
        
        if (_movementVelocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(_movementVelocity);
        }

        _animator.SetFloat(_animIDSpeed,_moveSpeed);
        _animator.SetFloat(_animIDMotionSpeed,inputMagnitude);
        
        if (_hasAnimator)
        {
            _animator.SetBool(_animIDGrounded, _cc.isGrounded);
        }
    }
    
    private void JumpAndGravity()
    {
        if (_cc.isGrounded)
        {
            _fallTimeoutDelta = FallTimeout;
            
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDJump, false);
                _animator.SetBool(_animIDFall, false);
            }
                        
            //stop dropping infinite when grounded
            if (_verticalVelocity.y < 0.0f)
            {
                _verticalVelocity.y = -2f;
            }

            if (_input.jump && _jumpTimeoutDelta <= 0.0f)
            {
                _verticalVelocity.y += Mathf.Sqrt(JumpHeight * -3.0f * _gravity);
                _animator.SetBool(_animIDJump, true);
            }
            
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
            else
            {
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDFall, true);
                }
            }
            
            _verticalVelocity.y += _gravity * Time.deltaTime;
            _input.jump = false;
        }
    }

    private void CalculateMovementEnemy()
    {
        if (Vector3.Distance(_targetPlayer.position, transform.position) >= _navMeshAgent.stoppingDistance)
        {
            _navMeshAgent.SetDestination(_targetPlayer.position);
            _animator.SetFloat(_animIDWalk,_navMeshAgent.speed);
            // Debug.Log(Vector3.Distance(_targetPlayer.position, transform.position));
        }
        else
        {
            _navMeshAgent.SetDestination(transform.position);
            _animator.SetFloat(_animIDWalk, 0f);
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
                    attackComboCount = 1;
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
                    if (Time.deltaTime < attackSlideDuraton + attackStartTime)
                    {
                        float timePassed = Time.time - attackStartTime;
                        float lerpTime = timePassed / attackSlideDuraton;
                        _movementVelocity = Vector3.Lerp(transform.forward * attackSlideSpeed, Vector3.zero, lerpTime);
                    }
                    if (_input.attack && _cc.isGrounded)
                    {
                        _attackAnimationDuration = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                        if (_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "Combo04" 
                            && _attackAnimationDuration > 0.5f && _attackAnimationDuration < 0.7f)
                        {
                            attackComboCount++;
                            _input.attack = false;
                            SwitchStateTo(CharacterState.Attacking);
                            CalculateMovementPlayer();
                        }
                        Debug.Log(_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name);
                    }
                }
                break;
            case CharacterState.Slide:
                break;
            case CharacterState.BeingHit:
                if (_impactOnPlayer.magnitude > 0.2f)
                {
                    _movementVelocity = _impactOnPlayer * Time.deltaTime;
                }
                _impactOnPlayer = Vector3.Lerp(_impactOnPlayer, Vector3.zero, Time.deltaTime * 5);
                break;
            case CharacterState.Dead:
                break;
        }

        if (isPlayer && CurrentState != CharacterState.Dead)
        {
            // move the player
            _cc.Move(_movementVelocity + _verticalVelocity * Time.deltaTime);
            _movementVelocity = Vector3.zero;
        }
    }

    public void SwitchStateTo(CharacterState newState)
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
                if (_damageCaster != null)
                {
                    DisableDamageCaster();
                }

                if (isPlayer)
                {
                    GetComponent<VFXPlayerController>().StopBlade();
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
                    Quaternion newRotation = Quaternion.LookRotation(_targetPlayer.position - transform.position);
                    transform.rotation = newRotation;
                }
                
                _animator.SetTrigger(_animIDAttack);
                attackStartTime = Time.time;
                break;
            case CharacterState.Sprint:
                break;
            case CharacterState.Slide:
                break;
            case CharacterState.BeingHit:
                _animator.SetTrigger(_animIDBeingHit);
                if (isPlayer)
                {
                    _isInvincible = true;
                    StartCoroutine(DelayCancelInvincible());
                }
                break;
            case CharacterState.Spawn:
                break;
            case CharacterState.Dead:
                _cc.enabled = false;
                _animator.SetTrigger(_animIDDead);
                break;
            case CharacterState.Roll:
                _animator.SetTrigger(_animIDRoll);
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
    
    IEnumerator DelayCancelInvincible()
    {
        yield return new WaitForSeconds(_invincibleDuration);
        _isInvincible = false;
    }


    public void AddImpact(Vector3 attackerPos, float force)
    {
        Vector3 impactDir = transform.position - attackerPos;
        impactDir.Normalize();
        impactDir.y = 0;
        _impactOnPlayer = impactDir * force;
    }

    public void EnableDamageCaster()
    {
        _damageCaster.EnableDamageCaster();
    }
    
    public void DisableDamageCaster()
    {
        _damageCaster.DisableDamageCaster();
    }
    
    public void RotateToTarget()
    {
        if (CurrentState != CharacterState.Dead)
        {
            transform.LookAt(_targetPlayer, Vector3.up);
        }
    }
}
