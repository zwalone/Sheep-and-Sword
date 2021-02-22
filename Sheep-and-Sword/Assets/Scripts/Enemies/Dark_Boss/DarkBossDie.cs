using UnityEngine;

public class DarkBossDie : MonoBehaviour
{
    [SerializeField]
    private GameObject Wall;

    private void FixedUpdate()
    {
        // Unlock way to next level after defeating Dark Boss:
        if (gameObject.GetComponent<Dark_Boss_Model>().HP <= 0)
            Wall.SetActive(false);
    }
}

