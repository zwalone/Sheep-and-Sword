using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potions : MonoBehaviour
{
    [SerializeField]
    private int heal = 50;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var model = collision.gameObject.GetComponent<PlayerModel>();
            Debug.Log("heal");
            if(model.HP != model.MaxHP)
            {
                model.HP += heal;
                Destroy(this.gameObject);

            }

        }
    }
}
