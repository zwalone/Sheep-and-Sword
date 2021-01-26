using UnityEngine;

public class CameraLineController : MonoBehaviour
{
    private GameObject mainCamera;

    private void Awake()
    {
        mainCamera = GameObject.Find("Main Camera");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            mainCamera.GetComponent<CameraTrackController>().LockCamera();
        }
    }
}
