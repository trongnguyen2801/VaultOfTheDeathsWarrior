using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Character_Input : MonoBehaviour
{
    [Header("CharacterFirstDesign Input Values")]
    public Vector2 move;
    public bool jump;
    public bool sprint;
    public bool roll;
    public bool attack;

    public void OnMove(InputValue value)
    {
        MoveInput(value.Get<Vector2>());
    }

    public void OnJump(InputValue value)
    {
        JumpInput(value.isPressed);
    }

    public void OnSprint(InputValue value)
    {
        SprintInput(value.isPressed);
    }
    
    public void OnRoll(InputValue value)
    {
        RollInput(value.isPressed);
    }

    public void OnAttack(InputValue value)
    {
        AttackInput(value.isPressed);
    }
    
    private void MoveInput(Vector2 moveDirections)
    {
        move = moveDirections;
    }

    private void JumpInput(bool jumpState) 
    {
        jump = jumpState;
    }
    
    private void RollInput(bool jumpState)
    {
        roll = jumpState;
    }
    
    private void SprintInput(bool sprintState)
    {
        sprint = sprintState;
    }
    
    private void AttackInput(bool attackState)
    {
        attack = attackState;
    }

    public void ClearCache()
    {
        attack = false;
        jump = false;
        sprint = false;
        roll = false;
    }
}
