using UnityEngine;

public class PotionController : MonoBehaviour
{
    // Amount of the health points that will be gained to the player:
    [SerializeField]
    private int heal = 50;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Player's components:
            var model = collision.gameObject.GetComponent<PlayerModel>();
            var controller = collision.gameObject.GetComponent<PlayerController>();
            var sound = collision.gameObject.GetComponent<SoundController>();

            if (model.HP != model.MaxHP)
            {
                // Make potion sound:
                sound.PlaySound(6);

                // Update player's health points:
                model.HP += heal;
                controller.UpdatePlayerHealthBar();

                // Remove potion from scene:
                Destroy(gameObject);
            }
        }
    }
}
