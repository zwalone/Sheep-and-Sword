using UnityEngine;

public class Worm_View : MonoBehaviour
{
    // List of animations:
    private enum Anim
    {
        Walk,
        Attack,
        Die,
        Hurt
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
    public void Attack() { ChangeAnimState(Anim.Attack); }
    public void Hurt() { ChangeAnimState(Anim.Hurt); }
    public void Die() { ChangeAnimState(Anim.Die); }
}
