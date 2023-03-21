using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponController : MonoBehaviour
{
    private HandController handController;
    [SerializeField] GameObject prefabBullet;
    [SerializeField] Transform targetRecoil; // Точка задания силы отталкивания
    [SerializeField] Text outputConditionBullet; // Вывод состоянии патрон
    [SerializeField] LeftHendController leftHendController; // Контроллер левой руки
    private GameObject player; // Ссылка на игрока
    private Rigidbody playerRigidbody; // Ссылка на гравитацию объекта
    private PlayerMove playerMove; // Ссылка на компонент передвижения


    // Компоненты создаваемой пули
    private GameObject currentBullet;
    private Bullet bullet;

    // Параметры гранаты
    [SerializeField] private Text grenadeTXT;
    [SerializeField] private Transform grenadeTransform; // Позиция спавна гранат
    [SerializeField] private WeaponData grenadeChatacter; // Хорактеристики гранаты
    [SerializeField] private GameObject grenadePrefab; // Префаб гранаты

    private float timeReloadGrenade; // Время до броска гранаты
    private float lastTimeReloadGrenade; // Время полследнего броска
    private int countGrenade; // Колличество гранат


    // Параметны оружия
    [SerializeField] private WeaponData[] weaponDatas;

    private Transform positionShoot; // Позиция спавна пули
    private Transform targetPositionLeftHand; // Позиция направления левой руки

    private float damage = 0; // Урон за выстрел 
    private float speedShoot = 0.001f; // Скорость стерльбы (секунды за выстел)
    private float timeReload = 0.001f; // Время перезарядки
    private int[] curentBullet = new int[2] { 0, 0 }; // Текущие патроны 0 - сколько в объеме 1 - максимальный объем обоймы
    private int[] allBullet = new int[2] { 0, 0 }; // Запас патрон 0 - текущий запас дополнительных пуль 1 - максимальный запас дополнителных пуль
    private float speedBullet = 0.1f; //Скорость пули
    private float timeBullet = 10f; // Время жизни пули
    private float recoilWeapon = 0f; // Отдача оружия
    private float gravityBullet = 0f; // Гравитация пули
    private int countBullet = 1; // Колличество пуль за выстрел
    float[] scatterBullet = new float[2] { 0f, 0f }; // Разброс пуль 0 - минимальный разброс, 1 - максимальный разброс
    private float splashDistans = 0f; // Сплеш урон
    private int countThroughShot = 0; // Колличество сквозных прострелов

    private float nextShoot; // Время до сделующего выстрела
    private float nextReload; // Время ожидания до перезарядки
    private bool reload; // Перезаряжается ли оружие
    private int curentWeapon; // Текушее оружие
    private int[] rangeWeapon = new int[2] {0, 0}; // Диапозон индексов оружий

    // Аудио настройки
    private AudioSource audioSource;
    private AudioClip shotFX;
    private bool audioEffect;

    void Start()
    {
        // Выставление параметров гранаты
        countGrenade = grenadeChatacter.CurentBullet[0];
        timeReloadGrenade = grenadeChatacter.SpeedShoot;
        ShowCountGrenade();
        audioSource = gameObject.GetComponent<AudioSource>();
        audioEffect = PlayerPrefs.GetInt("EffectStatus", 1) == 1 ? true : false;


        player = GameObject.FindGameObjectWithTag("Player");
        playerRigidbody = player.GetComponent<Rigidbody>();
        playerMove = player.GetComponent<PlayerMove>();
        rangeWeapon[1] = weaponDatas.Length-1;
        handController = gameObject.GetComponent<HandController>();
        ActivateWeapon(0);
    }

    
    void Update()
    {   
        // --- Стрельба ---
        // Отсчет перезарядки
        if (reload)
        {
            if (nextReload > 0)
                nextReload -= Time.deltaTime;
            else
            {
                nextReload = 0;
                reload = false;
                // Проверка на перзарядку полного магазина
                if (allBullet[0] >= curentBullet[1] - curentBullet[0])
                {
                    print("Полная перезарядка");
                    allBullet[0] -= (curentBullet[1] - curentBullet[0]);
                    curentBullet[0] = curentBullet[1];
                }
                // Если пуль не хаватает на полную перезарядку
                else
                {
                    print("Не полная перезарядка");
                    curentBullet[0] = allBullet[0];
                    allBullet[0] = 0;
                }
                ShowCounBullet(curentBullet[0], allBullet[0]);
            }
        }
        // Проверка на откат выстрела
        else if (nextShoot <= 0)
        {
            // Если зажата ЛКМ то происходит выстрел
            if (Input.GetMouseButton(0))
            {
                // Проверка на колличесто пуль
                if (curentBullet[0] > 0)
                {
                    Shoot();
                    nextShoot = speedShoot;
                    curentBullet[0]--;
                    ShowCounBullet(curentBullet[0], allBullet[0]);
                    // Если это была последная пуля, то запускается перезарядка
                    if (curentBullet[0] == 0 && allBullet[0] > 0)
                    {
                        nextReload = timeReload;
                        handController.Reload(nextReload);
                        reload = true;
                    }
                }
                // Запуск перезарядки оружия, если в дополнительных потрон больше 0
                else if (allBullet[0] > 0)
                {
                    nextReload = timeReload;
                    handController.Reload(nextReload);
                    reload = true;
                }
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                if (curentBullet[1] != curentBullet[0] && allBullet[0] > 0)
                {
                    // Бонус за не пустой мгазин перезарядки
                    nextReload = timeReload * 0.8f;
                    handController.Reload(nextReload);
                    reload = true;
                }
            }
        }
        else
        {
            nextShoot -= Time.deltaTime;
        }


        // --- Смена оружия ---

        // Проверка на вращение колеса
        // Вращение на следующее оружие
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            StopReloadWeapon(); // Перкращение перезарядки оружия при смене 
            if (curentWeapon>= rangeWeapon[1])
            {
                curentWeapon = rangeWeapon[0];
            }
            else
            {
                curentWeapon++;
            }
            ChangeWeapon(curentWeapon);
        }
        // Вращение на предыдущее оружие
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            StopReloadWeapon(); // Перкращение перезарядки оружия при смене
            if (curentWeapon <= rangeWeapon[0])
            {
                curentWeapon = rangeWeapon[1];
            }
            else
            {
                curentWeapon--;
            }
            ChangeWeapon(curentWeapon);
        }

        // Смена оружия по нажати кнопок
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StopReloadWeapon(); // Перкращение перезарядки оружия при смене
            ChangeWeapon(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StopReloadWeapon(); // Перкращение перезарядки оружия при смене
            ChangeWeapon(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            StopReloadWeapon(); // Перкращение перезарядки оружия при смене
            ChangeWeapon(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            StopReloadWeapon(); // Перкращение перезарядки оружия при смене
            ChangeWeapon(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            StopReloadWeapon(); // Перкращение перезарядки оружия при смене
            ChangeWeapon(4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            StopReloadWeapon(); // Перкращение перезарядки оружия при смене
            ChangeWeapon(5);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            StopReloadWeapon(); // Перкращение перезарядки оружия при смене
            ChangeWeapon(6);
        }


        // Граната
        if (Input.GetKeyDown(KeyCode.G))
        {
            if(lastTimeReloadGrenade + timeReloadGrenade <= Time.time && countGrenade > 0)
            {
                countGrenade--;
                lastTimeReloadGrenade = Time.time;
                ThrowGrenade();
                ShowCountGrenade();
            }
        }

    }

    void ChangeWeapon(int numWeapon)
    {
        StopReloadWeapon(); // Перкращение перезарядки оружия при смене
        if (rangeWeapon[0] <= numWeapon && numWeapon <= rangeWeapon[1])
        {
            curentWeapon = numWeapon;
            ActivateWeapon(curentWeapon);
        }
    }


    // Активация выбранного оружия и получение нужных параметров
    private void ActivateWeapon(int numWeapon)
    {
        for (int i = 0; i < weaponDatas.Length; i++)
        {
            // Если это выбранное оружие
            if (i == numWeapon)
            {
                weaponDatas[i].ObjebcWeapon.SetActive(true);

                positionShoot = weaponDatas[i].PositionShootBullet;
                targetPositionLeftHand = weaponDatas[i].TargetPositionLeftHand;

                damage = weaponDatas[i].Demage;
                speedShoot = weaponDatas[i].SpeedShoot;
                speedBullet = weaponDatas[i].SpeedBullet;
                timeBullet = weaponDatas[i].TimeBullet;
                recoilWeapon = weaponDatas[i].RecoilWeapon;
                gravityBullet = weaponDatas[i].GravityBullet;
                countBullet = weaponDatas[i].CountBullet;
                scatterBullet = weaponDatas[i].ScatterBullet;
                splashDistans = weaponDatas[i].SplashDistans;
                timeReload = weaponDatas[i].TimeReload;
                countThroughShot = weaponDatas[i].CountThroughShot;

                shotFX = weaponDatas[i].ShotFX;

                curentBullet = weaponDatas[i].CurentBullet;
                allBullet = weaponDatas[i].AllBullet;

                ShowCounBullet(curentBullet[0], allBullet[0]);

                nextShoot = speedShoot / 2; // Обнуление до следующего выстрела

                leftHendController.SetTarget(targetPositionLeftHand);
            }
            else
            {
                weaponDatas[i].ObjebcWeapon.SetActive(false);
            }
        }

        if (allBullet[0] > 0 && curentBullet[0] <= 0)
        {
            nextReload = timeReload;
            handController.Reload(nextReload);
            reload = true;
        }
    }


    // Выстрел
    void Shoot()
    {
        float[] ofsetRotationBullet = new float[countBullet]; // Смещение вращения пули
        float summOfset = 0f; // Сумма всего разброса 
        Vector3[] rotationBullets = new Vector3[countBullet]; // Вращение пули в эйлерах

        // Запись всех вращений пуль
        if (countBullet != 1)
        {
            for (int i = 0; i < countBullet; i++)
            {
                ofsetRotationBullet[i] = Random.Range(scatterBullet[0], scatterBullet[1]);
                summOfset += ofsetRotationBullet[i];
            }
        
            float lastRotation = -summOfset / 2;

            for (int i = 0; i < countBullet; i++)
            {
                rotationBullets[i].x = lastRotation + ofsetRotationBullet[i];
                lastRotation += ofsetRotationBullet[i];
            }
        }

        
        // Спавн пуль и передача аргументов
        for (int i = 0; i < countBullet; i++)
        {
            currentBullet = Instantiate(prefabBullet, positionShoot.position, Quaternion.Euler(positionShoot.transform.rotation.eulerAngles + rotationBullets[i]));
            bullet = currentBullet.GetComponent<Bullet>();
            bullet.SetBulletDamage(damage);
            bullet.SetBulletSpeed(speedBullet);
            bullet.SetBulletTime(timeBullet);
            bullet.SetGravity(gravityBullet);
            bullet.SetSplashDistans(splashDistans);
            bullet.SetCountThroughShot(countThroughShot);
        }

        // Оперделение относительного положения пули 
        Vector3 difference = player.transform.position - targetRecoil.position; // Раздница положений пули и туловищя игрока
        float coefficient = 1f;
        // Если игрок находится в прижке
        if (Mathf.Abs(playerRigidbody.velocity.y) > 0.1f)
        {
            print("В прыжке");
            coefficient = 0.65f;
        }
        difference.y = Mathf.Clamp(difference.y, -1, 1) * recoilWeapon * coefficient;
        difference.z = Mathf.Clamp(difference.z, -1, 1) * recoilWeapon * coefficient;
        playerMove.RecoilForse(difference);

        // Проигрывания звука выстрела
        if (audioEffect)
            audioSource.PlayOneShot(shotFX);
    }


    /// <summary>
    /// Отображает в текст состояния патрон
    /// </summary>
    /// <param name="curentBullet"> Текущие пули в запасе </param>
    /// <param name="allBullet"> Запас дополнительных пуль </param>
    private void ShowCounBullet(int curentBullet, int allBullet)
    {
        outputConditionBullet.text = $"{curentBullet}/{allBullet}";
    }

    // Прекращение перезарядки оружия
    private void StopReloadWeapon()
    {
        handController.StopReload(); // Перкращение перезарядки оружия при смене
        nextReload = 0;
    }

    // Спавн гранаты
    private void ThrowGrenade()
    {
        currentBullet = Instantiate(grenadePrefab, grenadeTransform.position, Quaternion.Euler(grenadeTransform.transform.rotation.eulerAngles));
        bullet = currentBullet.GetComponent<Bullet>();
        bullet.SetBulletDamage(grenadeChatacter.Demage);
        bullet.SetBulletSpeed(grenadeChatacter.SpeedBullet);
        bullet.SetBulletTime(grenadeChatacter.TimeBullet);
        bullet.SetGravity(grenadeChatacter.GravityBullet);
        bullet.SetSplashDistans(grenadeChatacter.SplashDistans);
    }

    
    private void ShowCountGrenade()
    {
        grenadeTXT.text = countGrenade.ToString();
    }


    public void GetWeaponBullet(int persentBullet)
    {
        int[] bullet = new int[2] { 0, 0 };
        foreach (WeaponData  weaponData in weaponDatas)
        {
            bullet[0] = weaponData.AllBullet[0] + (weaponData.AllBullet[1] * persentBullet / 100);
            bullet[1] = weaponData.AllBullet[1];
            weaponData.AllBullet = bullet;
        }
        allBullet = weaponDatas[curentWeapon].AllBullet;
        ShowCounBullet(curentBullet[0], allBullet[0]);
    }
}
