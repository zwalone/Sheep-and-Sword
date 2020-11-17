using UnityEngine;

public class PlayerModel : MonoBehaviour, IEntityModel
{
    [SerializeField]
    private int maxHP;
    public int MaxHP
    {
        get { return maxHP; }
        set { maxHP = value; }
    }

    [SerializeField]
    private int hp;
    public int HP
    {
        get { return hp; }
        set { hp = value; }
    }

    [SerializeField]
    private float speed;
    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }

    [SerializeField]
    private float jumpForce;
    public float JumpForce
    {
        get { return jumpForce; }
        set { jumpForce = value; }
    }

    [SerializeField]
    private int _dmg;
    public int Damage
    {
        get { return _dmg; }
        set { _dmg = value; }
    }
}
