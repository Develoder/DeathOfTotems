using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelaBonus : MonoBehaviour
{
    [SerializeField] private float heals = 15; // Дополнительное здоровье при подборе
    private bool getHeal; // Передал ли здоровье
    

    private void OnCollisionEnter(Collision collision)
    {
        if (getHeal)
            return;
        if (collision.gameObject.tag == "Player")
        {
            getHeal = true;
            collision.gameObject.GetComponent<HealPleyer>().Heal(heals);
            Destroy(gameObject);
        }
    }
}
