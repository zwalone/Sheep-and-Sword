using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    [SerializeField]
    private Vector3 offsetPosition = new Vector3(0, 2, -1);

    private void Update()
    {
        if (target == null)
        {
            Debug.LogWarning("Missing Target ref!", this);
            return;
        }

        transform.position = target.position + offsetPosition;
    }
}