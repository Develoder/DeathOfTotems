using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float damage; // Урон за выстрел
    private float speedBullet = 10; //Скорость пули
    private float timeBullet = 10f; // Время жизни пули
    private float gravityBullet = 0f; // Гравитация пули
    private float splashDistans = 0f; // Сплеш урон
    private int countThroughShot = 0; // Колличество сквозных прострелов

    private bool back; // Движение назад


    private bool usGravity; // Использовать ли гравитацию
    private new Rigidbody rigidbody;
    private Vector3 gravity; // Сила гравитации
    [SerializeField] private GameObject splashGbj; // Объект сплеша


    void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();
        Invoke("DestroyBullet", timeBullet);
    }


    void FixedUpdate()
    {
        if (usGravity)
        {
            gravity.y = -gravityBullet * 100 * Time.fixedDeltaTime;
            rigidbody.AddForce(gravity, ForceMode.Force);
        }
        if (back)
            transform.Translate(Vector3.back * speedBullet * Time.fixedDeltaTime);
        else
            transform.Translate(Vector3.forward * speedBullet * Time.fixedDeltaTime);
    }

    // Уничтожение пули
    public void DestroyBullet()
    {
        Destroy(gameObject);
    }


    // Установка урона пули
    public void SetBulletDamage(float damage)
    {
        this.damage = Mathf.Clamp(damage, 0, 100);
    }

    // Установка скорости пули
    public void SetBulletSpeed(float speed)
    {
        speedBullet = Mathf.Clamp(speed, 0f, 100f);
    }

    // Установка гравитация
    public void SetGravity(float gravityBullet)
    {
        this.gravityBullet = gravityBullet;
        if (gravityBullet != 0f)
        {
            usGravity = true;
        }
    }

    // Установка радиуса поражения пули
    public void SetSplashDistans(float splashDistans)
    {
        this.splashDistans = Mathf.Clamp(splashDistans, 0f, 200f);
    }

    // Установка количеств сквозных прострелов
    public void SetCountThroughShot(int countThroughShot)
    {
        this.countThroughShot = countThroughShot;
    }


    // Установка время жизни пули
    public void SetBulletTime(float time)
    {
        timeBullet = Mathf.Clamp(time, 0f, 15f);
    }



    // При столкновении отправляет запрос через интерфейс на получения урона
    private void OnTriggerEnter(Collider other)
    {
        // Урон через сплеш
        if (splashDistans > 0f)
        {
            TakingSplahDamge();
        }
        // Обыс
        else
        {
            TakingDemage(damage, other.gameObject);
        }

        // Проверка на количество оставшихся прострелов
        if (countThroughShot > 0)
        {
            countThroughShot--;
        }
        // Если закончились прострелы
        else
        {
            DestroyBullet();
        }
    }

    // Передача урона через сплеш
    private void TakingSplahDamge()
    {
        GameObject game = Instantiate(splashGbj, transform.position, Quaternion.identity);
        game.GetComponent<Splash>().SetDemage(damage);
        game.GetComponent<Splash>().SetSplashDistans(splashDistans);
    }

    // Передача урона пули
    private void TakingDemage(float damage, GameObject game)
    {
        var components = new List<ITakingDemage>();
        components.AddRange(game.GetComponents<ITakingDemage>());
        foreach (var component in components)
        {
            component.TakingDemage(damage);
        }
    }

    // Передача направления
    public void OnBack(bool back)
    {
        this.back = back;
    }
}
