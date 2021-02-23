using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    // Maximum amount of health points:
    [SerializeField]
    private int maxHP;
    public int MaxHP
    {
        get { return maxHP; }
        set { maxHP = value; }
    }

    // Current amount of health points:
    [SerializeField]
    private int hp;
    public int HP
    {
        get { return hp; }
        set {
            hp = value;
            if (hp > MaxHP) hp = MaxHP;
        }
    }

    // Value responsible for changing position (running, climbing):
    [SerializeField]
    private float speed;
    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }

    // Value responsible for changing position while jumping:
    [SerializeField]
    private float jumpForce;
    public float JumpForce
    {
        get { return jumpForce; }
        set { jumpForce = value; }
    }
}
