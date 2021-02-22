using UnityEngine;

public class Dark_Boss_View : MonoBehaviour
{
    // List of animations:
    private enum Anim
    {
        Attack1,
        Attack2,
        Hurt,
        Heal,
        Run,
        Dash,
        Die
    }

    private Anim currentState;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        currentState = Anim.Run;
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
    public void Walk() { ChangeAnimState(Anim.Run); }
    public void Dash() { ChangeAnimState(Anim.Dash); }
    public void Attack1() { ChangeAnimState(Anim.Attack1); }
    public void Attack2() { ChangeAnimState(Anim.Attack2); }
    public void Hurt() { ChangeAnimState(Anim.Hurt); }
    public void Die() { ChangeAnimState(Anim.Die); }
    public void Heal() { ChangeAnimState(Anim.Heal); }
}
