using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DarkBossDie : MonoBehaviour
{
    [SerializeField]
    private GameObject Wall;

    private void FixedUpdate()
    {
        if (gameObject.GetComponent<Dark_Boss_Model>().HP <= 0)
        {
            Wall.SetActive(false);
        }
    }
}

