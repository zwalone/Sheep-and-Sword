using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinotaurView : MonoBehaviour
{
    private enum Anim
    {
        WalkRight,
        AttackRight,
        Attack2Right,
        DieRight,
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

    public void WalkRight()
    {
        ChangeAnimState(Anim.WalkRight);
    }

    public void AttackRight()
    {
        ChangeAnimState(Anim.AttackRight);
    }

    public void Attack2Right()
    {
        ChangeAnimState(Anim.Attack2Right);
    }

    public void DieRight()
    {
        ChangeAnimState(Anim.DieRight);
    }

    public void GetDamage()
    {
        ChangeAnimState(Anim.GetDamage);
    }
}
