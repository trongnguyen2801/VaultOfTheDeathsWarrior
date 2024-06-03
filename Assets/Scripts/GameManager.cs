using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance => _instance;
    private static GameManager _instance;
    
    #region Animation Ids Variables

    public int animIDSpeed;
    public int animIDGrounded;
    public int animIDJump;
    public int animIDFall;
    public int animIDMotionSpeed;
    public int animIDWalk;
    public int animIDDead;
    public int animIDBeingHit;
    public int animIDAttack;
    public int animIDRoll;
    public int canAttack;

    #endregion

    private void Awake()
    {
        _instance = this;
        AssignAnimationIDs();
    }

    private void Start()
    {
    }
    private void AssignAnimationIDs()
    {
        animIDSpeed = Animator.StringToHash("Speed");
        animIDGrounded = Animator.StringToHash("Ground");
        animIDJump = Animator.StringToHash("Jump");
        animIDFall = Animator.StringToHash("Fall");
        animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        animIDWalk = Animator.StringToHash("Walk");
        animIDDead = Animator.StringToHash("Dead");
        animIDBeingHit = Animator.StringToHash("BeingHit");
        animIDAttack = Animator.StringToHash("Attack");
        animIDRoll = Animator.StringToHash("Roll");
    }
}
