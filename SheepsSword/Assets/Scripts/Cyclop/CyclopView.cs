using UnityEngine;

public class CyclopView : MonoBehaviour
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

    public void DieRight()
    {
        ChangeAnimState(Anim.DieRight);
    }
    public void DieLeft()
    {
        ChangeAnimState(Anim.DieLeft);
    }
}
