using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBigGun : MonoBehaviour
{
    [SerializeField] GameObject prefabBullet;

    private Transform playerTransform;
    private Transform curentTransform;

    [SerializeField] private Transform transformGun; // Трансформ левой пушки
    [SerializeField] private Transform positionShootBullet; // Позиция выстрела левой пушки

    private WeaponData weaponData;

    // Параметры оружия
    private float demage; // Урон за выстрел
    private float speedShoot = 0.001f; // Скорость стрельбы (секунда/патрон)
    private float speedBullet = 0.1f; // Скорость пули
    private float timeBullet = 0.1f; // Время жизни пули

    // Параметры логики оружия
    [Header("Parametrs cannon")]
    [SerializeField] private float maxRotationGun; // Максимальное вращение пушки
    [SerializeField] private float timeBeforeShoot; // Время ожидания до выстрела
    [SerializeField] private float[] timeSleeps = new float[2]; // Время до активации блока
    private float timeSpeep; // ожидание до активации
    private float timeNextShoot; // Время до следующего выстрела
    private float timeNextBeforeShoot; // Вредмя ожидания до выстрела
    public float differenceRotarion; // Раздница вращения
    private bool startRotation; // Началось ли вращение
    private bool left; // С лева ли игрок
    private bool isLeft; // С левой стороны находится пушка

    // Позиция орудия
    private Vector3 difference; // Раздница векторов для орудия и игроком
    public Vector3 сurentRotationGun; // Текущее вращение левого орудия
    public Vector3 targetRotationGun; // Целевое вращение рудия


    private AudioSource audioSource;
    private AudioClip shoot;
    private bool audioEffect;


    private void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        
        audioEffect = PlayerPrefs.GetInt("EffectStatus", 1) == 1 ? true : false;

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        curentTransform = transform;

        weaponData = gameObject.GetComponent<WeaponData>();

        demage = weaponData.Demage;
        speedShoot = weaponData.SpeedShoot;
        speedBullet = weaponData.SpeedBullet;
        timeBullet = weaponData.TimeBullet;

        shoot = weaponData.ShotFX;

        timeNextShoot = speedShoot;
        timeNextBeforeShoot = timeBeforeShoot;

        timeSpeep = Random.Range(timeSleeps[0], timeSleeps[1]);

        // Проверка на нахождение пушки с левой стороны
        if (transform.position.z - positionShootBullet.position.z > 0)
        {
            isLeft = true;
            сurentRotationGun.x = 0f;
        }
        else
            сurentRotationGun.x = 180f;
    }



    void Update()
    {
        // Не активна пушка
        if (timeSpeep > 0)
        {
            timeSpeep -= Time.deltaTime;
            return;
        }

        if (timeNextShoot <= 0)
        {
            // Если время поворота до ожидаемого места закончилось, производится выстрел
            if (timeNextBeforeShoot <= 0)
            {
                // Если вращение пушки не соответсвует положению игрока, то выстрел не производится
                if ((difference.z < 0f) == isLeft)
                {
                    // Подготовка к выстреллу
                    StartingShoot();
                    startRotation = false;
                    timeNextBeforeShoot = timeBeforeShoot;
                }
                else
                {
                    startRotation = false;
                    print("Положение игрока и пушки не равны");
                    timeNextShoot = speedShoot + timeBeforeShoot;
                    timeNextBeforeShoot = timeBeforeShoot;
                }
            }
            // Поворот пушки к последнему месту появления игрока
            else
            {
                // Срабатывает один раз за выстрел
                if (!startRotation)
                {
                    difference = playerTransform.position - curentTransform.position; // Раздница положений игрока и пушки
                    targetRotationGun.x = Mathf.Atan2(difference.y, -difference.z) * Mathf.Rad2Deg;// Определение вращение к игроку

                    // Определяем по какой стороне вращяться будет пушка
                    // Левая сторона
                    if (difference.z < 0f)
                    {
                        left = true;
                        differenceRotarion = Mathf.Abs(targetRotationGun.x - сurentRotationGun.x);
                    }
                    // Правая сторона
                    else
                    {
                        left = false;
                        // Нормализуем ускрорение
                        if (targetRotationGun.x / Mathf.Abs(targetRotationGun.x) != сurentRotationGun.x / Mathf.Abs(сurentRotationGun.x))
                        {
                            differenceRotarion = Mathf.Abs(targetRotationGun.x - сurentRotationGun.x) / 5;
                        }
                        else
                        {
                            differenceRotarion = Mathf.Abs(targetRotationGun.x - сurentRotationGun.x);
                        }
                    }
                    startRotation = true;
                }

                // Вращение левой пушки
                if (left && isLeft)
                {
                    targetRotationGun.x = Mathf.Clamp(targetRotationGun.x, -maxRotationGun, maxRotationGun);
                    сurentRotationGun.x = Mathf.MoveTowardsAngle(сurentRotationGun.x, targetRotationGun.x, differenceRotarion / timeBeforeShoot * Time.deltaTime);
                    transformGun.rotation = Quaternion.Euler(сurentRotationGun.x, 0f, 0f);
                }
                // Вращение правой пушки
                else if (!left && !isLeft)
                {
                    if (targetRotationGun.x > 0)
                        targetRotationGun.x = Mathf.Clamp(targetRotationGun.x, 180 - maxRotationGun, 180);
                    else
                        targetRotationGun.x = Mathf.Clamp(targetRotationGun.x, -180, maxRotationGun - 180);

                    сurentRotationGun.x = Mathf.MoveTowardsAngle(сurentRotationGun.x, targetRotationGun.x, differenceRotarion / timeBeforeShoot * Time.deltaTime);
                    transformGun.rotation = Quaternion.Euler(сurentRotationGun.x, 0f, 0f);
                }


                timeNextBeforeShoot -= Time.deltaTime;
            }
        }
        else
        {
            timeNextShoot -= Time.deltaTime;
        }

    }


    // Определение выстрелла
    void StartingShoot()
    {
        Vector3 rotarionBullet = new Vector3();
        if (difference.z < 0)
        {
            rotarionBullet.x = сurentRotationGun.x;
            StartCoroutine(Shoot(timeBeforeShoot, positionShootBullet, rotarionBullet));
        }
        else
        {
            rotarionBullet.x = сurentRotationGun.x;
            StartCoroutine(Shoot(timeBeforeShoot, positionShootBullet, rotarionBullet));
        }
        timeNextShoot = speedShoot + timeBeforeShoot;
    }

    public void OnDestroyBigGyn()
    {
        Destroy(gameObject);
    }


    IEnumerator Shoot(float time, Transform transformSoot, Vector3 rotarionBullet)
    {
        yield return new WaitForSeconds(time);
        // Проверка на разрешенность проигрывания звуков
        if (audioEffect)
            audioSource.PlayOneShot(shoot);
        GameObject currentBullet = Instantiate(prefabBullet, transformSoot.position, Quaternion.Euler(rotarionBullet));
        Bullet bullet = currentBullet.GetComponent<Bullet>();
        bullet.SetBulletDamage(demage);
        bullet.SetBulletSpeed(speedBullet);
        bullet.SetBulletTime(timeBullet);
        bullet.OnBack(true);
    }
}
