﻿using UnityEngine;

public class CyclopModel : MonoBehaviour
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

    // Value responsible for tracking player:
    [SerializeField]
    private float raycastDistance;
    public float RaycastDistance
    {
        get { return raycastDistance; }
        set { raycastDistance = value; }
    }

    // Object spawned when cyclop is attacking:
    [SerializeField]
    private GameObject laser;
    public GameObject Laser
    {
        get { return laser; }
        set { laser = value; }
    }
}
