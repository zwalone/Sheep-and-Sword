using UnityEngine;

public class SkeletonModel : MonoBehaviour
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
        set { hp = value; }
    }

    // Value responsible for changing position:
    [SerializeField]
    private float speed;
    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }
}
