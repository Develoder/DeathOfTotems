using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    //// Настройки камеры
    [SerializeField] private float smoothTime; // Скорость перемещения

    [SerializeField] private float horizontalOfset; // Горизонтальное смещение
    [SerializeField] private float verticaleOfset; // Вертикальное смещение


    private Transform playerTransform;
    private Transform cameraTransform;

    private Vector3 targetPosition; // Целевая позиция
    private Vector3 newPosition; // Новая позиция
    private Vector3 velocity = Vector3.zero;
    

    // Настройки тряски камеры
    public enum ShakeMode { OnlyX, OnlyY, OnlyZ, XY, XZ, XYZ }; // Тряска по осям

    private static Transform tr; // Ссылка на трансформ этого боъекта
    private static float elapsed; // Время до остановки тряски
    private static float i_Duration; // Продолжительность тряски
    private static float i_Power; // Сила тряски
    private static float percentComplete;
    private static ShakeMode i_Mode; // Модификация тряски
    private static Vector3 originalPos;
    private Vector3 rnd; // Рандомное направление тряски камеры

    void Start()
    {
        percentComplete = 1;
        tr = Camera.main.GetComponent<Transform>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").gameObject.transform;
        cameraTransform = Camera.main.gameObject.transform;

        newPosition = transform.position;

    }

    public  void Shake(float duration, float power)
    {
        if (percentComplete == 1) originalPos = tr.localPosition;
        i_Mode = ShakeMode.XYZ;
        elapsed = 0;
        i_Duration = duration;
        i_Power = power;
    }

    public  void Shake(float duration, float power, ShakeMode mode)
    {
        if (percentComplete == 1) originalPos = tr.localPosition;
        i_Mode = mode;
        elapsed = 0;
        i_Duration = duration;
        i_Power = power;
    }

    void Update()
    {
        // Перемещение камеры за игроком
        targetPosition.x = transform.position.x;
        targetPosition.y = playerTransform.position.y + verticaleOfset;
        targetPosition.z = playerTransform.position.z + horizontalOfset;

        newPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        transform.position = newPosition;


        // Тряска камеры
        if (elapsed < i_Duration)
        {
            elapsed += Time.deltaTime;
            percentComplete = elapsed / i_Duration;
            percentComplete = Mathf.Clamp01(percentComplete);
            rnd = Random.insideUnitSphere * i_Power * (1f - percentComplete);

            switch (i_Mode)
            {
                case ShakeMode.XYZ:
                    tr.localPosition = originalPos + rnd;
                    break;
                case ShakeMode.OnlyX:
                    tr.localPosition = originalPos + new Vector3(rnd.x, 0, 0);
                    break;
                case ShakeMode.OnlyY:
                    tr.localPosition = originalPos + new Vector3(0, rnd.y, 0);
                    break;
                case ShakeMode.OnlyZ:
                    tr.localPosition = originalPos + new Vector3(0, 0, rnd.z);
                    break;
                case ShakeMode.XY:
                    tr.localPosition = originalPos + new Vector3(rnd.x, rnd.y, 0);
                    break;
                case ShakeMode.XZ:
                    tr.localPosition = originalPos + new Vector3(rnd.x, 0, rnd.z);
                    break;
            }
        }
    }
}
