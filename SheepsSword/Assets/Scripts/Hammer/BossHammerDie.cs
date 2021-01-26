using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHammerDie : MonoBehaviour
{
    [SerializeField]
    private GameObject Wall;

    private void FixedUpdate()
    {
        if (gameObject.GetComponent<Hammer_Model>().HP <= 0)
        {
            Wall.SetActive(false);
        }
    }
}
