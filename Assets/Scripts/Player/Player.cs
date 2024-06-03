using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour, IDamageable
{
    public float MaxHealth { get; set; }
    public float CurrentHealth { get; set; }
    private Character _cc;
    private CharacterController _characterController;
    public Character_Input input;
    private Animator _animator;
    
    public float _moveSpeed;
    public float _runSpeed = 2.5f;
    public float _sprintSpeed = 4f;
    public float JumpHeight = 2.2f;
    
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
    
    private DamageCaster _damageCaster;
    private bool _hasAnimator;

    private bool _isInvincible;
    private float _invincibleDuration = 1f;

    private void Awake()
    {
        input = GetComponent<Character_Input>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        MaxHealth = 200f;
        CurrentHealth = MaxHealth;
        _cc = GetComponent<Character>();
        
        _characterController = GetComponent<CharacterController>();
        _damageCaster = GetComponentInChildren<DamageCaster>();
        _hasAnimator = TryGetComponent(out _animator);

        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;
        _isInvincible = false; // tesst
    }
    
    public void CalculateMovementPlayer()
    {
        if (input.attack && _characterController.isGrounded)
        {
            _cc.SwitchStateTo(Character.CharacterState.Attacking);
            return;
        }
        
        if (input.roll && _characterController.isGrounded)
        {
            _cc.SwitchStateTo(Character.CharacterState.Roll);
            return;
        }
        
        _moveSpeed = input.sprint ? _sprintSpeed : _runSpeed;
        
        if (input.move == Vector2.zero) _moveSpeed = 0.0f;
        if (_characterController.isGrounded && _verticalVelocity.y < 0) _verticalVelocity.y = 0;
        
        float inputMagnitude = input.move.magnitude;
        
        _movementVelocity = new Vector3(input.move.x,0f,input.move.y);
        _movementVelocity.Normalize();
        _movementVelocity = Quaternion.Euler(0, -45f, 0) * _movementVelocity;
        _movementVelocity *= _moveSpeed * Time.deltaTime;
        
        if (_movementVelocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(_movementVelocity);
        }

        _animator.SetFloat(GameManager.Instance.animIDSpeed,_moveSpeed);
        _animator.SetFloat(GameManager.Instance.animIDMotionSpeed,inputMagnitude);
        
        if (_hasAnimator)
        {
            _animator.SetBool(GameManager.Instance.animIDGrounded, _characterController.isGrounded);
        }
    }

    public void PlayerMove()
    {
        if (_cc.CurrentState != Character.CharacterState.Dead)
        {
            // move the player
            _characterController.Move(_movementVelocity + _verticalVelocity * Time.deltaTime);
            _movementVelocity = Vector3.zero;
        } 
    }
    
    
    public void JumpAndGravity()
    {
        if (_characterController.isGrounded)
        {
            _fallTimeoutDelta = FallTimeout;
            
            if (_hasAnimator)
            {
                _animator.SetBool(GameManager.Instance.animIDJump, false);
                _animator.SetBool(GameManager.Instance.animIDFall, false);
            }
                        
            //stop dropping infinite when grounded
            if (_verticalVelocity.y < 0.0f)
            {
                _verticalVelocity.y = -2f;
            }

            if (input.jump && _jumpTimeoutDelta <= 0.0f)
            {
                _verticalVelocity.y += Mathf.Sqrt(JumpHeight * -3.0f * _gravity);
                _animator.SetBool(GameManager.Instance.animIDJump, true);
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
                    _animator.SetBool(GameManager.Instance.animIDFall, true);
                }
            }
            
            _verticalVelocity.y += _gravity * Time.deltaTime;
            input.jump = false;
        }
    }

    public void ApplyDamage(float dmg, Vector3 posAttack = new Vector3())
    {
        CurrentHealth -= dmg;
        Debug.Log("player apply damage" + CurrentHealth);

        if (CurrentHealth < 0)
        {
            Debug.Log("death");
            _cc.SwitchStateTo(Character.CharacterState.Dead);
        }
        
        _cc.SwitchStateTo(Character.CharacterState.BeingHit);
        AddImpact(posAttack,5f);

        float perHealth = CurrentHealth / MaxHealth;
        ProfileManager.Instance.SetHealthAndMana(perHealth,1f);
    }

    public void SlidePlayerAttack()
    {                    
        if (Time.deltaTime < attackSlideDuraton + attackStartTime)
        {
            float timePassed = Time.time - attackStartTime;
            float lerpTime = timePassed / attackSlideDuraton;
            _movementVelocity = Vector3.Lerp(transform.forward * attackSlideSpeed, Vector3.zero, lerpTime);
        }
    }

    public void PlayerAttackCombo()
    {
        if (input.attack && _characterController.isGrounded)
        {
            _attackAnimationDuration = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            if (_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "Combo04" 
                && _attackAnimationDuration > 0.5f && _attackAnimationDuration < 0.7f)
            {
                input.attack = false;
                _cc.SwitchStateTo(Character.CharacterState.Attacking);
                CalculateMovementPlayer();
            }
            // Debug.Log(_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name);
        }
    }

    public void PlayerBeingHit()
    {
        if (_impactOnPlayer.magnitude > 0.2f)
        {
            _movementVelocity = _impactOnPlayer * Time.deltaTime;
        }
        _impactOnPlayer = Vector3.Lerp(_impactOnPlayer, Vector3.zero, Time.deltaTime * 5);
    }

    public void StopBladeAnimation()
    {
        GetComponent<VFXPlayerController>().StopBlade();
    }
    
    public void AddImpact(Vector3 attackerPos, float force)
    {
        Vector3 impactDir = transform.position - attackerPos;
        impactDir.Normalize();
        impactDir.y = 0;
        _impactOnPlayer = impactDir * force;
    }

    public void InviciblePlayer()
    {
        _isInvincible = true;
        StartCoroutine(DelayCancelInvincible());
    }
    
    IEnumerator DelayCancelInvincible()
    {
        yield return new WaitForSeconds(_invincibleDuration);
        _isInvincible = false;
    }

    public void Die()
    {
        _characterController.enabled = false;
    }
}
