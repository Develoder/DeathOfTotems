using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainEnemy : MonoBehaviour
{
    [SerializeField] private List<GameObject> enemyObject;
    [SerializeField] private float length; // Длина перемещения вниз блоков 
    [SerializeField] private float distanseActivet; // Дистанция активности врага

    private List<GameObject> deleteEnemyObject = new List<GameObject>(); // Удаленные части
    private Transform playerTrahsform;
    private bool active = true; // Активен ли враг


    private void Start()
    {
        playerTrahsform = GameObject.FindGameObjectWithTag("Player").transform;
    }


    private void Update()
    {
        if (Mathf.Abs(transform.position.z - playerTrahsform.position.z) > distanseActivet)
        {
            if (active)
            {
                foreach (GameObject game in enemyObject)
                {
                    game.SetActive(false);
                }
                active = false;
            }
        }
        else if (!active)
        {
            foreach (GameObject game in enemyObject)
            {
                game.SetActive(true);
            }
            active = true;
        }
    }



    // Сещение частей тотемов по их смерти
    public void DestroyPartEnemy(GameObject gameObject)
    {
        foreach (GameObject enemyObj in enemyObject)
        {
            if (enemyObj == gameObject)
            {
                deleteEnemyObject.Add(enemyObj);
                enemyObject.Remove(enemyObj);
                ShangeBodyEnemy(deleteEnemyObject[deleteEnemyObject.Count - 1]);
                break;
            }
        }
    }


    /// <summary>
    /// Пермещение блоков на удалеенное место
    /// </summary>
    /// <param name="game"> Объект к тоторому нужно переместить остальные объекты </param>
    private void ShangeBodyEnemy(GameObject game)
    {
        List<GameObject> upEnemy = new List<GameObject>(); // Объекты которые нужно опустить
        // Определение объектов которые нужно опустить вниз
        for (int i = 0; i < enemyObject.Count; i++)
        {
            if (enemyObject[i].transform.position.y > game.transform.position.y)
            {
                upEnemy.Add(enemyObject[i]);
            }
        }

        // Перемещение объектов вниз
        foreach (GameObject gameDown in upEnemy)
        {
            gameDown.transform.position = new Vector3(gameDown.transform.position.x, gameDown.transform.position.y - length, gameDown.transform.position.z); 
        }
    }
}
