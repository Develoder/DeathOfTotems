using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HealPleyer : MonoBehaviour, ITakingDemage
{
    [Header("Sound")]
    private AudioSource audioSource;
    [SerializeField] private AudioClip HitFX;
    [SerializeField] private AudioClip GetHeal;
    [SerializeField] private AudioClip GetAmunition;


    [Header("Heal parametrs")]
    [SerializeField] private float maxHitPoint = 100;
    [SerializeField] private Image healBarImage;
    private string nameSceneGameOver = "GameOver";


    private float curentHitPoint;
    private CameraMove cameraMove;
    private bool audioEffect;

    private void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        audioEffect = PlayerPrefs.GetInt("EffectStatus", 1) == 1 ? true : false;
        cameraMove = Camera.main.transform.parent.GetComponent<CameraMove>();
        curentHitPoint = maxHitPoint;
    }


    private void ShowHealt()
    {
        healBarImage.fillAmount = curentHitPoint / maxHitPoint;
    }



    public void TakingDemage(float dagame)
    {
        
        // Смерть игрока
        if (curentHitPoint < dagame || curentHitPoint - dagame <= 0)
        {
            SceneManager.LoadScene(nameSceneGameOver);
        }
        // Получение урона
        else
        {
            SoundFX(0);
            curentHitPoint -= dagame;
            cameraMove.Shake(0.3f, 0.15f);
            ShowHealt();
        }
    }

    public void Heal(float heal)
    {
        print(heal);
        curentHitPoint += heal;
        if (curentHitPoint > maxHitPoint)
        {
            curentHitPoint = maxHitPoint;
        }
        ShowHealt();
        SoundFX(1);
    }

    // Проигрывание звука
    public void SoundFX(int numSound)
    {
        if (audioEffect)
        {
            switch (numSound)
            {
                case 0:
                    audioSource.PlayOneShot(HitFX);
                    break;
                case 1:
                    audioSource.PlayOneShot(GetHeal);
                    break;
                case 2:
                    audioSource.PlayOneShot(GetAmunition);
                    break;
            }
        }
    }
}
