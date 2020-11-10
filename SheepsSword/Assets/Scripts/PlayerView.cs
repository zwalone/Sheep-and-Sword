using UnityEngine;

public class PlayerView : MonoBehaviour
{
    public bool LookRight { get; set; }

    private enum Anim
    {
        Idle,
        Run,
        Jump,
        Fall,
        Crouch,
        Attack1,
        SomerSault
    }

    private Anim currentState;
    public Animator GetAnimator { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        LookRight = true;
        GetAnimator = GetComponent<Animator>();
        currentState = Anim.Idle;
    }

    private void ChangeAnimState(Anim state)
    {
        //Stop the same animation playing
        if (currentState == state) return;

        currentState = state;
        GetAnimator.Play(currentState.ToString());
    }

    public void Idle() { ChangeAnimState(Anim.Idle); }
    public void Run() { ChangeAnimState(Anim.Run); }
    public void Jump() { ChangeAnimState(Anim.Jump); }
    public void Fall() { ChangeAnimState(Anim.Fall); }
    public void Crouch() { ChangeAnimState(Anim.Crouch); }
    public void Attack1() { ChangeAnimState(Anim.Attack1); }
    public void SomerSault() { ChangeAnimState(Anim.SomerSault); }
}
