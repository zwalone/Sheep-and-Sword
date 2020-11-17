using UnityEngine;

public class MinotaurModel : MonoBehaviour, IEntityModel
{
    [SerializeField]
    private int _hp;
    public int HP
    {
        get { return _hp; }
        set { _hp = value; }
    }

    [SerializeField]
    private float _speed;
    public float Speed
    {
        get { return _speed; }
        set { _speed = value; }
    }

    [SerializeField]
    private int _dmg;
    public int Damage
    {
        get { return _dmg; }
        set { _dmg = value; }
    }
}
