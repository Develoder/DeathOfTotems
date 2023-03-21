using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateSound : MonoBehaviour
{
    [SerializeField] private AudioSource[] controllerAtivityAudioSource; // Работа с включением и отключением аудио клипов

    private bool audioMusic; // Состояние активации музыки

    void Start()
    {
        audioMusic = PlayerPrefs.GetInt("MusicStatus", 1) == 1 ? true : false;

        if (!audioMusic) {
            foreach (AudioSource аudioSource in controllerAtivityAudioSource)
            {
                аudioSource.volume = 0;
            }
        }
    }
}
