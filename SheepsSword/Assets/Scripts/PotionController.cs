using UnityEngine;
using UnityEngine.UI;

public class PotionController : MonoBehaviour
{
    [SerializeField]
    private int heal = 50;

    private Image playerHealthBar;

    private void Awake()
    {
        playerHealthBar = GameObject.Find("PlayerHealthBar_Fill").GetComponent<Image>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var model = collision.gameObject.GetComponent<PlayerModel>();
            if(model.HP != model.MaxHP)
            {
                collision.gameObject.GetComponent<SoundController>().PlaySound(6);
                model.HP += heal;
                playerHealthBar.fillAmount = model.HP / model.MaxHP;
                Destroy(gameObject);
            }
        }
    }
}
