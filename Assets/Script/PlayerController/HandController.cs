using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    [SerializeField] private Transform targetPosition; // В какой точке должна находится рука
    [SerializeField] private RotationBody rotationBody;
    [SerializeField] private Transform rotationReload; // Вращение объекта для слежения за ним
    [SerializeField] private Transform targetReload; // Слежеине за точкой вращения

    public float offset;

    private Vector3 cursor; // Положение курсора
    private Vector3 difference; // Раздница векторов для вращение руки
    private Vector3 differenceRotat; // Раздница векторов для вращения персонажа
    private float rotationX; // Вращение объекта по оси X
    private float rotationZ; // Вращение объекта по оси Z

    // Перезарядка
    private bool reload; // Производится ли перезарядка
    private float timeReload; // Время перезарядки
    private float curentTimeReload; // Оставшиеся время до конца перезарядки
    private Vector3 rotateReload; // Вращение руки во время перезарядки


    void Update()
    {
        transform.position = targetPosition.position; // Постоянное слежение за точкой таргета
        
        // Вращение руки к курсору
        cursor = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.x - transform.position.x));
        difference = (reload? targetReload.position : cursor) - transform.position;
        differenceRotat = cursor -transform.position;

        rotationX = Mathf.Atan2(-difference.y, difference.z) * Mathf.Rad2Deg;
        // Вращение руки к курсору
        transform.rotation = Quaternion.Euler(rotationX + offset, 0f, rotationZ);
        // Поворот туловищя в сторонну курсора
        if (Mathf.Abs(Mathf.Atan2(-differenceRotat.y, differenceRotat.z) * Mathf.Rad2Deg) > 90)
        {
            rotationBody.RotatiBody(180);
            rotationZ = 180f; 
        }
        else
        {
            rotationBody.RotatiBody(0);
            rotationZ = 0f;
        }


        // Перезарядка оружия
        if (reload)
        {
            if (curentTimeReload <= 0)
            {
                reload = false;
            }
            curentTimeReload -= Time.deltaTime;
            rotationReload.Rotate(rotateReload.x, 0, 0);
        }

    }


    /// <summary>
    /// Перезарядка оружия
    /// </summary>
    /// <param name="timeReload"> Время перезарядки </param>
    public void Reload(float timeReload)
    {
        this.timeReload = timeReload;
        curentTimeReload = timeReload;
        reload = true;

        rotateReload.x = 360 / timeReload * Time.deltaTime;
        rotationReload.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, rotationZ, 0));
    }

    public void StopReload()
    {
        reload = false;
    }
}
