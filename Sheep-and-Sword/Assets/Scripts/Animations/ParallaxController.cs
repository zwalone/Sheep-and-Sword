using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    // Camera:
    public GameObject cam;

    // Parallax parameters:
    private float length;
    private float startpos;
    public float parallaxEffect;

    void Start()
    {
        startpos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void FixedUpdate()
    {
        float temp = (cam.transform.position.x * (1 - parallaxEffect));
        float dist = (cam.transform.position.x * parallaxEffect);

        // Change position of the background:
        transform.position = new Vector3(startpos + dist, transform.position.y, transform.position.z);

        // Calculate new position (partially):
        if (temp > startpos + length) startpos += length;
        else if (temp < startpos - length) startpos -= length;
    }
}
