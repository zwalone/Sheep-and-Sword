using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demon_Axe_View : MonoBehaviour
{
    private enum Anim
    {
        Walk,
        Attack1,
        Dead,
        Hit,
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

    public void Attack()
    {
        ChangeAnimState(Anim.Attack1);
    }

    public void TakeDamage()
    {
        ChangeAnimState(Anim.Hit);
    }


    public void Die()
    {
        ChangeAnimState(Anim.Dead);
    }
}
