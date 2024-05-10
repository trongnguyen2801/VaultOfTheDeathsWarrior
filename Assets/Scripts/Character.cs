using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    
    public enum CharacterState
    {
        Normal, Attacking, Dead, BeingHit, Slide,Spawn,Sprint,
    }
    public CharacterState CurrentState;

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _characterInput = GetComponent<Character_Input>();
    }

    private void CalculateMovementVelocity()
    {
        if (_characterInput.mouseButtonDown && _cc.isGrounded)
        {
            SwitchStateTo(CharacterState.Attacking);
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

    private void FixedUpdate()
    {
        switch (CurrentState)
        {
            case CharacterState.Normal:
                CalculateMovementVelocity();
                break;
            case CharacterState.Attacking:
                if (Time.deltaTime < attackSlideDuraton + attackStartTime)
                {
                    float timePassed = Time.time - attackStartTime;
                    float lerpTime = timePassed / attackSlideDuraton;
                    _movementVelocity = Vector3.Lerp(transform.forward * attackSlideSpeed, Vector3.zero, lerpTime);
                }
                if (_characterInput.mouseButtonDown && _cc.isGrounded)
                {
                    _attackAnimationDuration = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                    if (_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "Male Attack 3" && _attackAnimationDuration > 0.7f)
                    {
                        _characterInput.mouseButtonDown = false;
                        SwitchStateTo(CharacterState.Attacking);
                        CalculateMovementVelocity();
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
        }
        
        switch (newState)
        {
            case CharacterState.Normal:
                break;
            case CharacterState.Attacking:
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
        }

        CurrentState = newState;
    }
    
    public void AttackAnimationEnds()
    {
        SwitchStateTo(CharacterState.Normal);
    }
}
