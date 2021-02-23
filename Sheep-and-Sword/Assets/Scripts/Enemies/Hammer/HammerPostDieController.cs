using UnityEngine;

public class HammerPostDieController : MonoBehaviour
{
    // Map fragments:
    [SerializeField]
    private GameObject Wall;
    [SerializeField]
    private GameObject[] Hole;

    // General:
    private bool itIsDone = false;
    private AudioSource[] sounds;

    private void Start()
    {
        sounds = GameObject.Find("Music").GetComponents<AudioSource>();
    }

    private void FixedUpdate()
    {
        // After Hammer's HP drops 0...
        if (gameObject.GetComponent<Hammer_Model>().HP <= 0 && !itIsDone)
        {
            // Update action status:
            itIsDone = true;

            // Hide map fragment, show the way to the end of the game:
            Wall.SetActive(false);
            foreach (var h in Hole) h.SetActive(false);

            // Volume the boss music down:
            GameObject.Find("DialogPoint").GetComponent<LastFightDialogShowController>().BossMusicVolumeDown();

            // Start making hearbeat sound:
            sounds[3].Play();
        }
    }
}
