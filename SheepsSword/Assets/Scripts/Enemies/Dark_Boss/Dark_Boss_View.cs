using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dark_Boss_View : MonoBehaviour
{
    private enum Anim
    {
        Dark_Attack1,
        Dark_Attack2,
        Dark_Idle, //hit
        Dark_Magic, //heal
        Dark_Run, //walk
        Dark_Dash,
        Die
    }

    private Anim _currentState;

    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        _animator = this.GetComponent<Animator>();
        _currentState = Anim.Dark_Run;
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
        ChangeAnimState(Anim.Dark_Run);
    }

    public void Dash()
    {
        ChangeAnimState(Anim.Dark_Dash);
    }

    public void Attack2()
    {
        ChangeAnimState(Anim.Dark_Attack2);
    }

    public void Attack1()
    {
        ChangeAnimState(Anim.Dark_Attack1);
    }

    public void TakeDamage()
    {
        ChangeAnimState(Anim.Dark_Idle);
    }

    public void Die()
    {
        ChangeAnimState(Anim.Die);
    }

    public void Heal()
    {
        ChangeAnimState(Anim.Dark_Magic);
    }
}
