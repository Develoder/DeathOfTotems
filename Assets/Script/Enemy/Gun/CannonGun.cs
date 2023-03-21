using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonGun : MonoBehaviour
{
    [SerializeField] GameObject prefabBullet;

    private Transform playerTransform;
    private Transform curentTransform;

    [SerializeField] private Transform transformLeftGun; // Трансформ левой пушки
    [SerializeField] private Transform positionShootLeftBullet; // Позиция выстрела левой пушки
    [SerializeField] private Transform transformRightGun; // Трансформ правой пушки
    [SerializeField] private Transform positionShootRightBullet; // Позиция выстрела правой пушки

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
    private bool left; // Последнее вращение происходило с лева
    private float differenceRotarion; // Раздница вращения
    private bool startRotation; // Началось ли вращение

    // Позиция орудия
    private Vector3 difference; // Раздница векторов для орудия и игроком
    public Vector3 сurentRotationLeftGun; // Текущее вращение левого орудия
    public Vector3 сurentRotationRightGun; // Текущее варщение правого орудия
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
    }

    

    void Update()
    {
        if (timeSpeep > 0)
        {
            timeSpeep -= Time.deltaTime;
            return;
        }
        if (timeNextShoot <= 0)
        {
            // Если время поворота до ожидаемого места закончилось
            if (timeNextBeforeShoot <= 0)
            {
                // Подготовка к выстреллу
                StartingShoot();
                startRotation = false;
                timeNextBeforeShoot = timeBeforeShoot;
            }
            else
            {
                // Срабатывает один раз за выстрел
                if (!startRotation)
                {
                    difference = playerTransform.position - curentTransform.position; // Раздница положений игрока и пушки
                    targetRotationGun.x = Mathf.Atan2(difference.y, -difference.z) * Mathf.Rad2Deg;// Определение вращение к игроку

                    // Определяем какая пушка будет вращяться
                    if (difference.z < 0f)
                    {
                        left = true;
                        differenceRotarion = Mathf.Abs(targetRotationGun.x - сurentRotationLeftGun.x);
                    }
                    else
                    {
                        left = false;
                        // Нормализуем ускрорение
                        if (targetRotationGun.x / Mathf.Abs(targetRotationGun.x) != сurentRotationRightGun.x / Mathf.Abs(сurentRotationRightGun.x))
                        {
                            differenceRotarion = Mathf.Abs(targetRotationGun.x - сurentRotationRightGun.x) / 5;
                        }
                        else
                        {
                            differenceRotarion = Mathf.Abs(targetRotationGun.x - сurentRotationRightGun.x);
                        }
                    }
                    startRotation = true;
                }

                // Вращение левой пушки
                if (left)
                {
                    targetRotationGun.x = Mathf.Clamp(targetRotationGun.x, -maxRotationGun, maxRotationGun);
                    сurentRotationLeftGun.x = Mathf.MoveTowardsAngle(сurentRotationLeftGun.x, targetRotationGun.x, differenceRotarion / timeBeforeShoot * Time.deltaTime);
                    transformLeftGun.rotation = Quaternion.Euler(сurentRotationLeftGun.x, 0f, 0f);
                }
                // Вращение правой пушки
                else
                {
                    if (targetRotationGun.x > 0)
                        targetRotationGun.x = Mathf.Clamp(targetRotationGun.x, 180 - maxRotationGun, 180);
                    else
                        targetRotationGun.x = Mathf.Clamp(targetRotationGun.x, -180, maxRotationGun - 180);

                    сurentRotationRightGun.x = Mathf.MoveTowardsAngle(сurentRotationRightGun.x, targetRotationGun.x, differenceRotarion / timeBeforeShoot * Time.deltaTime);
                    transformRightGun.rotation = Quaternion.Euler(сurentRotationRightGun.x, 0f, 0f);
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
            rotarionBullet.x = сurentRotationLeftGun.x;
            StartCoroutine(Shoot(timeBeforeShoot, positionShootLeftBullet, rotarionBullet));
        }
        else
        {
            rotarionBullet.x = сurentRotationRightGun.x;
            StartCoroutine(Shoot(timeBeforeShoot, positionShootRightBullet, rotarionBullet));
        }
        timeNextShoot = speedShoot + timeBeforeShoot;
    }

    public void OnDestroyCannonGyn()
    {
        Destroy(gameObject);
    }


    IEnumerator Shoot(float time, Transform transformSoot, Vector3 rotarionBullet)
    {
        yield return new WaitForSeconds(time);
        if(audioEffect)
            audioSource.PlayOneShot(shoot);
        GameObject currentBullet = Instantiate(prefabBullet, transformSoot.position, Quaternion.Euler(rotarionBullet));
        Bullet bullet = currentBullet.GetComponent<Bullet>();
        bullet.SetBulletDamage(demage);
        bullet.SetBulletSpeed(speedBullet);
        bullet.SetBulletTime(timeBullet);
        bullet.OnBack(true);
    }
}
