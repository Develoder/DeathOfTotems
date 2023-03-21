using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Splash : MonoBehaviour
{
    private float damage; // Урон по сплешу
    private float splashDistans; // Радиус взрыва
    private float lifeTime; // Время жизни
    

    [SerializeField] private ParticleSystem[] particleSystems = new ParticleSystem [3];
    [SerializeField] private AudioClip boomFX;
    private AudioSource audioSource;
    private bool audioEffect;

    private List<GameObject> objCollision = new List<GameObject>(); // Лист со всеми столкнувшимися объектов
    private float lastTime; // Последнее время 
    private bool firstStart = true; // Первый проход по колайдерам
    private bool takingDemage; // Отправлен ли урон

    private void Start()
    {
        Camera.main.transform.parent.GetComponent<CameraMove>().Shake(0.1f, 0.4f);
        audioSource = gameObject.GetComponent<AudioSource>();
        audioEffect = PlayerPrefs.GetInt("EffectStatus", 1) == 1 ? true : false;
        if (audioEffect)
            audioSource.PlayOneShot(boomFX);


        foreach (ParticleSystem particle in particleSystems)
        {
            if (particle.startLifetime > lifeTime)
            {
                lifeTime = particle.startLifetime;
            }
            particle.transform.localScale = new Vector3(splashDistans, splashDistans, splashDistans);
        }
        transform.localScale = new Vector3(splashDistans, splashDistans, splashDistans);
        

        Invoke("DestroyObj", lifeTime);
    }


    // Занесение всех объектов имеющих здровье в список
    private void OnTriggerStay(Collider other)
    {
        if (firstStart)
        {
            firstStart = false;
            lastTime = Time.time;
        }
        if (lastTime < Time.time)
        { 
            if (!takingDemage)
            {
                TakingDamages();
                takingDemage = true;
            }
            return;
        }
        else if (!objCollision.Contains(other.gameObject))
        {
            objCollision.Add(other.gameObject);
            lastTime = Time.time;
        }
    }


    // Нанесение урона
    private void TakingDamages()
    {
        var components = new List<ITakingDemage>();
        for (int i = 0; i < objCollision.Count; i++)
        {
            components = new List<ITakingDemage>();
            components.AddRange(objCollision[i].gameObject.GetComponents<ITakingDemage>());
            foreach (var component in components)
            {
                component.TakingDemage(damage);
            }
        }
    }


    // Уничтожение объектов
    private void DestroyObj()
    {
        print("Колличество найденных объектов " + objCollision.Count);
        Destroy(gameObject);
    }


    public void SetSplashDistans(float splashDistans)
    {
        this.splashDistans = splashDistans;
    }

    // Получение урона
    public void SetDemage(float damage)
    {
        this.damage = damage;
    }

}
