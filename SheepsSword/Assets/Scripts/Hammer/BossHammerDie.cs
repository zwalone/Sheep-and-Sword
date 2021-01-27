using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHammerDie : MonoBehaviour
{
    [SerializeField]
    private GameObject Wall;

    [SerializeField]
    private GameObject[] Hole;

    private void FixedUpdate()
    {
        if (gameObject.GetComponent<Hammer_Model>().HP <= 0)
        {
            Wall.SetActive(false);
            foreach (var h in Hole)
            {
                h.SetActive(false);
            }
        }
    }
}
