using System.Collections;
using UnityEngine;

public class CameraTrackController : MonoBehaviour
{
    // Tracking:
    [SerializeField]
    private Transform target;
    [SerializeField]
    private Vector3 offsetPosition = new Vector3(0, 2, -1);
    
    // Stopping tracking:
    private bool isLocked = false;

    // Lights on:
    private SpriteRenderer darkEffect;
    public float darkEffectSpeed = 0.01f;
    public Coroutine lightsOn;

    // Turn the lights on:
    private void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        darkEffect = gameObject.GetComponentInChildren<SpriteRenderer>();
        lightsOn = StartCoroutine(LightsOn());
    }

    // Keep tracking the target:
    private void Update()
    {
        if (target == null) return;
        if (!isLocked) transform.position = target.position + offsetPosition;
    }

    // Change state responsible for tracking (permanently):
    public void LockCamera() { isLocked = true; }

    // Enabling dark camera effect:
    public IEnumerator LightsOff() 
    {
        while (darkEffect.color.a < 1)
        {
            darkEffect.color = new Color(darkEffect.color.r, darkEffect.color.g,
                darkEffect.color.b, darkEffect.color.a + 0.01f);
            yield return new WaitForSeconds(darkEffectSpeed);
        }
    }

    // Disabling dark camera effect:
    public IEnumerator LightsOn()
    {
        while (darkEffect.color.a > 0)
        {
            darkEffect.color = new Color(darkEffect.color.r, darkEffect.color.g, 
                darkEffect.color.b, darkEffect.color.a - 0.01f);
            yield return new WaitForSeconds(darkEffectSpeed);
        }
    }
}
