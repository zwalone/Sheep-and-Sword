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
        CrouchWalk,
        Attack1,
        Attack2,
        Attack3,
        AirAttack,
        CrouchAttack,
        SomerSault,
        Climb,
        WallSlide,
        WallHold,
        Die,
        Hurt
    }

    private Anim currentState;
    public Animator GetAnimator { get; private set; }

    // Start is called before the first frame update
    void Awake()
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
    public void CrouchWalk() { ChangeAnimState(Anim.CrouchWalk); }
    public void Attack1() { ChangeAnimState(Anim.Attack1); }
    public void Attack2() { ChangeAnimState(Anim.Attack2); }
    public void Attack3() { ChangeAnimState(Anim.Attack3); }
    public void AirAttack() { ChangeAnimState(Anim.AirAttack); }
    public void CrouchAttack() { ChangeAnimState(Anim.CrouchAttack); }
    public void SomerSault() { ChangeAnimState(Anim.SomerSault); }
    public void Climb() { ChangeAnimState(Anim.Climb); }
    public void WallSlide() { ChangeAnimState(Anim.WallSlide); }
    public void WallHold() { ChangeAnimState(Anim.WallHold); }
    public void Die() { ChangeAnimState(Anim.Die); }
    public void Hurt() { ChangeAnimState(Anim.Hurt); }
}
