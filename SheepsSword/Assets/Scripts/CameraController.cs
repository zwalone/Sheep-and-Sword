using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    [SerializeField]
    private Vector3 offsetPosition = new Vector3(0, 2, -1);

    private bool isLocked = false;

    private void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (target == null)
        {
            Debug.LogWarning("Missing Target ref!", this);
            return;
        }
        
        if (!isLocked) transform.position = target.position + offsetPosition;
    }

    public void LockCamera() { isLocked = true; }
}