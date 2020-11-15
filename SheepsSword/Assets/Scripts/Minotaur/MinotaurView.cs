using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinotaurView : MonoBehaviour
{
    private enum Anim
    {
        WalkRight,
        WalkLeft,
        AttackRight,
        Attack2Right,
        Attack2Left,
        AttackLeft,
        DieRight,
        DieLeft,
        GetDamage
    }

    private Anim _currentState;

    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        _animator = this.GetComponent<Animator>();
        _currentState = Anim.WalkRight;
    }

    private void ChangeAnimState(Anim state)
    {
        //Stop the same animation playing
        if (_currentState == state) return;

        _currentState = state;

        _animator.Play(_currentState.ToString());
    }

    public void WalkLeft()
    {
        ChangeAnimState(Anim.WalkLeft);
    }

    public void WalkRight()
    {
        ChangeAnimState(Anim.WalkRight);
    }

    public void AttackRight()
    {
        ChangeAnimState(Anim.AttackRight);
    }

    public void AttackLeft()
    {
        ChangeAnimState(Anim.AttackLeft);
    }

    public void Attack2Right()
    {
        ChangeAnimState(Anim.Attack2Right);
    }

    public void Attack2Left()
    {
        ChangeAnimState(Anim.Attack2Left);
    }

    public void DieRight()
    {
        ChangeAnimState(Anim.DieRight);
    }
    public void DieLeft()
    {
        ChangeAnimState(Anim.DieLeft);
    }
    public void GetDamage()
    {
        ChangeAnimState(Anim.GetDamage);
    }
}
