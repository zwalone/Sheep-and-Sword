using UnityEngine;
using UnityEngine.UI;

public class HitBoxController : MonoBehaviour
{
    public string target = "";
    public int damage = 0;
    private GameObject enemyHealthBarFill;
    private GameObject enemyHealthBar;

    private void Awake()
    {
        enemyHealthBar = GameObject.Find("EnemyHealthBar");
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
            if (target != "Player" && collision.gameObject != null && enemyHealthBar != null)
            {
                CancelInvoke(nameof(HideEnemyHealthBar));
                enemyHealthBar.SetActive(true);
                enemyHealthBarFill = GameObject.Find("EnemyHealthBar_Fill");
                enemyHealthBarFill.GetComponent<Image>().fillAmount = (float)collision.gameObject.GetComponentInParent<IEntityController>().ReturnCurrentHP()
                    / collision.gameObject.GetComponentInParent<IEntityController>().ReturnMaxHP();
                Invoke(nameof(HideEnemyHealthBar), 5.0f);
            }
            else if (target == "Player" && enemyHealthBar != null)
            {
                CancelInvoke(nameof(HideEnemyHealthBar));
                enemyHealthBar.SetActive(true);
                enemyHealthBarFill = GameObject.Find("EnemyHealthBar_Fill");
                enemyHealthBarFill.GetComponent<Image>().fillAmount = (float)gameObject.GetComponentInParent<IEntityController>().ReturnCurrentHP()
                    / gameObject.GetComponentInParent<IEntityController>().ReturnMaxHP();
                Invoke(nameof(HideEnemyHealthBar), 5.0f);
            }

            // Hide health bar if enemy died:
            if (enemyHealthBarFill != null && enemyHealthBarFill.GetComponent<Image>().fillAmount == 0) HideEnemyHealthBar();
        }
    }

    private void HideEnemyHealthBar() { enemyHealthBar.SetActive(false); }
}