using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyModel : MonoBehaviour
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
    private float _raycastDistance;

    public float RaycastDistance
    {
        get { return _raycastDistance; }
        set { _raycastDistance = value; }
    }

    [SerializeField]
    private GameObject laser;

    public GameObject Laser
    {
        get { return laser; }
        set { laser = value; }
    }

}
