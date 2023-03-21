using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour, ITakingDemage
{
    // Задаваеммые переменные
    [Header("Characteristic box")]
    [Range(0f, 1000f)]
    private float maxHitPoint = 100f; // Максимальное здоровье

    [Space(3)]

    // Ссылки на бонусные передмены
    [Header("Spawn prefab")]
    [SerializeField] private GameObject[] prefabBonus;

    [SerializeField] private int[] rangeCountBonus = new int[2];

    // Остольные переменные
    private float hitPoint; // Очки здоровья

    private HealthBar healthBar; // Ссылка на дочерний объект с компонетом healthBar
    private bool destroy; // Уничтожен ли объект


    private void Start()
    {
        hitPoint = maxHitPoint;
        healthBar = GetComponentInChildren<HealthBar>();
    }


    // Уничтожение объекта
    private void DestroyBox()
    {
        if (destroy) return;
        destroy = true;
        GameObject[] bonus = new GameObject[Random.Range(rangeCountBonus[0], rangeCountBonus[1])];
        for (int i = 0; i < bonus.Length; i++)
        {
            bonus[i] = Instantiate(prefabBonus[Random.Range(0, prefabBonus.Length)], transform.position + new Vector3(0f, 0.5f, 0f), Quaternion.identity);
            bonus[i].GetComponent<Rigidbody>().AddForce(new Vector3(0, Random.Range(0.02f, 0.1f), Random.Range(0.02f, 0.1f)), ForceMode.Impulse);
        }
        Destroy(gameObject);
    }


    // Получение урона
    public void TakingDemage(float dagame)
    {
        hitPoint -= dagame;
        healthBar.DisplayHealthBar(hitPoint, maxHitPoint);
        if (hitPoint <= 0)
        {
            DestroyBox();
        }
    }
}
