using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyView : MonoBehaviour
{

    private enum Anim
    {
        WalkRight,
        WalkLeft,
        AttackRight,
        AttackLeft,
        DieRight,
        DieLeft
    }

    private Anim _curretState;

    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        _animator = this.GetComponent<Animator>();
        _curretState = Anim.WalkRight;
    }

    private void ChangeAnimState(Anim state)
    {
        //Stop the same animation playing
        if (_curretState == state) return;

        _curretState = state;

        _animator.Play(_curretState.ToString());
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

    public void DieRight()
    {
        ChangeAnimState(Anim.DieRight);
    }
    public void DieLeft()
    {
        ChangeAnimState(Anim.DieLeft);
    }
}
