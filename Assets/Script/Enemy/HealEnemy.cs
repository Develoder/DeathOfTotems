using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealEnemy : MonoBehaviour, ITakingDemage
{
    [SerializeField] private float maxHitPoint = 100f; // Максимальное здоровье
    private float curentHela;

    public UnityEvent Destroy; // Смерть врага
    public UnityEvent HitDamage; // Нанесение урона


    private HealthBar healthBar;
    private MainEnemy mainEnemy;


    private void Start()
    {
        curentHela = maxHitPoint;
        healthBar = GetComponentInChildren<HealthBar>();
        mainEnemy = gameObject.transform.parent.GetComponent<MainEnemy>();
    }

    
    public void TakingDemage(float dagame)
    {
        // Смерть врага
        if (dagame >= curentHela)
        {
            mainEnemy.DestroyPartEnemy(gameObject);
            gameObject.GetComponent<BoxCollider>().enabled = false;
            Destroy.Invoke();
        }
        // Нанесение урона
        else
        {
            curentHela -= dagame;
            healthBar.DisplayHealthBar(curentHela, maxHitPoint);
            HitDamage.Invoke();
        }
    }
}
