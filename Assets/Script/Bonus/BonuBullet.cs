using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonuBullet : MonoBehaviour
{
    [SerializeField] private int persentBullet = 10; // Дополнительные пули процентно от максимального запаса
    private bool getBullet; // Передал ли здоровье


    private void OnCollisionEnter(Collision collision)
    {
        if (getBullet)
            return;
        if (collision.gameObject.tag == "Player")
        {
            getBullet = true;
            foreach(Transform child in collision.transform)
            {
                if (child.name == "Rigth hand")
                {
                    child.gameObject.GetComponent<WeaponController>().GetWeaponBullet(persentBullet);
                }
            }
            collision.gameObject.GetComponent<HealPleyer>().SoundFX(2);
            Destroy(gameObject);
        }
    }
}
