using UnityEngine;

public class OutroCloudsController : MonoBehaviour
{
    public float SpeedClouds = 2f;

    // Animate the clouds:
    void Update()
    {
        transform.Translate(Vector3.right * Time.deltaTime * SpeedClouds);
    }
}
