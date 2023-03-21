using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleSoundController : MonoBehaviour
{
    [SerializeField] private Toggle toggleMusic;
    [SerializeField] private Toggle toggleEffect;

    private bool audioMusic; // Состояние активации музыки
    private bool audioEffect; // Состояние активации звуков

    void Start()
    {
        audioMusic = PlayerPrefs.GetInt("MusicStatus", 1) == 1? true : false;
        audioEffect = PlayerPrefs.GetInt("EffectStatus", 1) == 1 ? true : false;

        toggleMusic.isOn = audioMusic;
        toggleEffect.isOn = audioEffect;
    }

    // Изменение состояния музыки
    public void OnClickMusic()
    {
        audioMusic = toggleMusic.isOn;
        PlayerPrefs.SetInt("MusicStatus", audioMusic? 1 : 0);
    }

    public void OnClickEffect()
    {
        audioEffect = toggleEffect.isOn;
        PlayerPrefs.SetInt("EffectStatus", audioEffect ? 1 : 0);
    }
}
