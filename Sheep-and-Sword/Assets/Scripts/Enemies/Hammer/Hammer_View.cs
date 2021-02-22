using UnityEngine;

public class Hammer_View : MonoBehaviour
{
    // List of animations:
    private enum Anim
    {
        Attack1,
        Attack2,
        Attack3,
        Dash,
        Die,
        Hurt,
        Walk
    }

    private Anim currentState;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        currentState = Anim.Walk;
    }

    private void ChangeAnimState(Anim state)
    {
        // Do nothing if new action is the same as previous one:
        if (currentState == state) return;

        // Show new animation:
        currentState = state;
        animator.Play(currentState.ToString());
    }

    // Change animations:
    public void Walk() { ChangeAnimState(Anim.Walk); }
    public void Dash() { ChangeAnimState(Anim.Dash); }
    public void Attack1() { ChangeAnimState(Anim.Attack1); }
    public void Attack2() { ChangeAnimState(Anim.Attack2); }
    public void Attack3() { ChangeAnimState(Anim.Attack3); }
    public void Hurt() { ChangeAnimState(Anim.Hurt); }
    public void Die() { ChangeAnimState(Anim.Die); }
}
