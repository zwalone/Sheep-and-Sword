using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer_View : MonoBehaviour
{
    private enum Anim
    {
        Attack1,
        Attack_Spinner,
        Dash,
        Dead,
        Idle_Strip,
        Jump,
        Taunt,
        Walk
    }

    private Anim _currentState;

    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        _animator = this.GetComponent<Animator>();
        _currentState = Anim.Walk;
    }

    private void ChangeAnimState(Anim state)
    {
        //Stop the same animation playing
        if (_currentState == state) return;

        _currentState = state;

        _animator.Play(_currentState.ToString());
    }

    public void Walk()
    {
        ChangeAnimState(Anim.Walk);
    }

    public void Dash()
    {
        ChangeAnimState(Anim.Dash);
    }

    public void Attack2()
    {
        ChangeAnimState(Anim.Jump);
    }

    public void Attack()
    {
        ChangeAnimState(Anim.Attack1);
    }
    public void AttackSpinner()
    {
        ChangeAnimState(Anim.Attack_Spinner);
    }

    public void TakeDamage()
    {
        ChangeAnimState(Anim.Taunt);
    }

    public void Idle()
    {
        ChangeAnimState(Anim.Idle_Strip);
    }

    public void Die()
    {
        ChangeAnimState(Anim.Dead);
    }
}
