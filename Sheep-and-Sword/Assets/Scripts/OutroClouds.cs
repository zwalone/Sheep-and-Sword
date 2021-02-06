using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutroClouds : MonoBehaviour
{
    public float SpeedClouds = 2f;

    void Update()
    {
        this.transform.Translate(Vector3.right * Time.deltaTime * SpeedClouds);
    }
}
