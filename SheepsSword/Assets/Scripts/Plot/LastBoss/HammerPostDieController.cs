using System.Collections;
using UnityEngine;

public class HammerPostDieController : MonoBehaviour
{
    [SerializeField]
    private GameObject Wall;

    [SerializeField]
    private GameObject[] Hole;

    private bool itIsDone = false;
    private AudioSource[] sounds;

    private void Start()
    {
        sounds = GameObject.Find("Music").GetComponents<AudioSource>();
    }

    private void FixedUpdate()
    {
        if (gameObject.GetComponent<Hammer_Model>().HP <= 0 && !itIsDone)
        {
            itIsDone = true;
            Wall.SetActive(false);
            foreach (var h in Hole) h.SetActive(false);
            StartCoroutine(VolumeDown());
            sounds[3].Play();
        }
    }

    private IEnumerator VolumeDown()
    {
        AudioSource music = sounds[0];
        while (music.volume > 0)
        {
            music.volume -= 0.01f;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
