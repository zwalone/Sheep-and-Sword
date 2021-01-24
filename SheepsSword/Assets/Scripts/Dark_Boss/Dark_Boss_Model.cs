using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dark_Boss_Model : MonoBehaviour
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
}
