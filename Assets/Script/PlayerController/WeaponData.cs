using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponData : MonoBehaviour
{
    [Header("Parametrs position weapon")]

    [Tooltip("Объект который отображает оружие")]
    [SerializeField] private GameObject objebcWeapon;
    public GameObject ObjebcWeapon
    {
        get
        {
            return objebcWeapon;
        }
        protected set { }
    }

    [Tooltip("Точка к которой направленна левая рука")]
    [SerializeField] private Transform targetPositionLeftHand;
    public Transform TargetPositionLeftHand
        {
            get
            {
                return targetPositionLeftHand;
            }
            protected set { }
        }

    [Tooltip("Позиция выстрела пули")]
    [SerializeField] private Transform positionShootBullet;
    public Transform PositionShootBullet
        {
            get
            {
                return positionShootBullet;
            }
            protected set { }
        }

    [Tooltip("Звук выстрела")]
    [SerializeField] private AudioClip shotFX;
    public AudioClip ShotFX
    {
        get
        {
            return shotFX;
        }
        protected set { }
    }

    [Space(2)]

    [Header("Parametrs configuration bullet")]
    [Tooltip("Урон за выстрел")]
    [Range(0f, 200f)] [SerializeField] private float demage;
    public float Demage
        {
            get
            {
                return demage;
            }
            protected set { }
        }

    [Tooltip("Скорость стрельбы (секунда/патрон)")]
    [Range(0.001f, 20f)] [SerializeField] private float speedShoot = 0.001f;
    public float SpeedShoot
        {
            get
            {
                return speedShoot;
            }
            protected set { }
        }

    [Tooltip("Текущие пули в обойме / максимальная обойма")]
    [Range(0, 500)] [SerializeField] private int[] curentBullet = new int[2] {0,0};
    public int[] CurentBullet
    {
        get
        {
            return curentBullet;
        }
        set
        {
            curentBullet = value;
        }
    }

    [Tooltip("Всего пуль в запасе / максимаьное коллсичество")]
    [Range(0, 501)] [SerializeField] private int[] allBullet = new int[2] { 0, 0 };
    public int[] AllBullet
    {
        get
        {
            return allBullet;
        }
        set
        {
            allBullet[0] = Mathf.Clamp(value[0], 0, allBullet[1]);
        }
    }

    [Tooltip("Время перезарядки")]
    [Range(0.001f, 20f)] [SerializeField] private float timeReload = 0.001f;
    public float TimeReload
    {
        get
        {
            return timeReload;
        }
        protected set { }
    }

    [Tooltip("Скорость пули")]
    [Range(0.1f, 50f)] [SerializeField] private float speedBullet = 0.1f;
    public float SpeedBullet
        {
            get
            {
                return speedBullet;
            }
            protected set { }
        }

    [Tooltip("Время жизни пули")]
    [Range(0.1f, 100f)] [SerializeField] private float timeBullet = 0.1f;
    public float TimeBullet
        {
            get
            {
                return timeBullet;
            }
            protected set { }
        }

    [Tooltip("Отдача оружия")]
    [Range(0f, 15f)] [SerializeField] private float recoilWeapon = 0f;
    public float RecoilWeapon
    {
        get
        {
            return recoilWeapon;
        }
        protected set { }
    }

    [Tooltip("Сила гравитации пули")]
    [Range(0f, 20f)] [SerializeField] private float gravityBullet;
    public float GravityBullet
        {
            get
            {
                return gravityBullet;
            }
            protected set { }
        }

    [Tooltip("Колличество пуль за выстрел")]
    [Range(1, 100)] [SerializeField] private int countBullet = 1;
    public int CountBullet
        {
            get
            {
                return countBullet;
            }
            protected set { }
        }

    [Tooltip("Разброс пули")]
    [Range(0f, 180f)] [SerializeField] private float[] scatterBullet = new float[2] { 0f, 0f};
    public float[] ScatterBullet
    {
        get
        {
            return scatterBullet;
        }
        protected set { }
    }

    [Tooltip("Область поражения")]
    [Range(0f, 30f)] [SerializeField] private float splashDistans;
    public float SplashDistans
        {
            get
            {
                return splashDistans;
            }
            protected set { }
        }

    [Tooltip("Колличество сквозных прострелов")]
    [Range(0, 50)] [SerializeField] private int countThroughShot;
    public int CountThroughShot
    {
        get
        {
            return countThroughShot;
        }
        protected set { }
    }
}
