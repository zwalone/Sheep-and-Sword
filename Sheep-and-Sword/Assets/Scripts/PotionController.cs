using UnityEngine;
using UnityEngine.UI;

public class PotionController : MonoBehaviour
{
    [SerializeField]
    private int heal = 50;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var model = collision.gameObject.GetComponent<PlayerModel>();
            var controller = collision.gameObject.GetComponent<PlayerController>();
            var sound = collision.gameObject.GetComponent<SoundController>();
            if(model.HP != model.MaxHP)
            {
                sound.PlaySound(6);
                model.HP += heal;
                controller.UpdatePlayerHealthBar();
                Destroy(gameObject);
            }
        }
    }
}
