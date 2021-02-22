using UnityEngine;
using UnityEngine.UI;

public class HitBoxController : MonoBehaviour
{
    // For checking if player has hit enemy or enemy has hit player:
    public string target = "";

    // The amount of health points that will be decreased:
    public int damage = 0;

    // Special boolean for bosses:
    public bool isBoss = false;

    // For displaying:
    private GameObject enemyHealthBarFill;
    private GameObject enemyHealthBar;

    private void Awake()
    {
        enemyHealthBar = GameObject.Find("UI").transform.Find("EnemyHealthBar").gameObject;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Checking tag (checking layer doesn't work):
        if (collision.CompareTag(target))
        {
            // If player is sliding - you can't hit him:
            if (target == "Player" && collision.gameObject.GetComponent<PlayerController>().IsSliding) return;

            // Apply damage:
            if (!gameObject.GetComponentInParent<IEntityController>().IsDead 
                && !gameObject.GetComponentInParent<IEntityController>().IsHurting)
                collision.gameObject.GetComponentInParent<IEntityController>().TakeDamage(damage);

            // Update health bar:
            enemyHealthBar.SetActive(true);
            if (target != "Player" && collision.gameObject != null)
            {
                // Don't hide enemy's health bar after hit:
                CancelInvoke(nameof(HideEnemyHealthBar));

                enemyHealthBarFill = GameObject.Find("EnemyHealthBar_Fill");
                enemyHealthBarFill.GetComponent<Image>().fillAmount 
                    = (float)collision.gameObject.GetComponentInParent<IEntityController>().ReturnCurrentHP()
                    / collision.gameObject.GetComponentInParent<IEntityController>().ReturnMaxHP();

                // Hide enemy's health bar after 5 seconds if he is not boss:
                if (!isBoss) Invoke(nameof(HideEnemyHealthBar), 5.0f);
            }
            else if (target == "Player")
            {
                // Don't hide enemy's health bar after being hit:
                CancelInvoke(nameof(HideEnemyHealthBar));

                enemyHealthBarFill = GameObject.Find("EnemyHealthBar_Fill");
                enemyHealthBarFill.GetComponent<Image>().fillAmount 
                    = (float)gameObject.GetComponentInParent<IEntityController>().ReturnCurrentHP()
                    / gameObject.GetComponentInParent<IEntityController>().ReturnMaxHP();

                // Hide enemy's health bar after 5 seconds if he is not boss:
                if (!isBoss) Invoke(nameof(HideEnemyHealthBar), 5.0f);
            }

            // Hide health bar if enemy died:
            if (enemyHealthBarFill.GetComponent<Image>().fillAmount == 0) HideEnemyHealthBar();
        }
    }

    private void HideEnemyHealthBar() { enemyHealthBar.SetActive(false); }
}
