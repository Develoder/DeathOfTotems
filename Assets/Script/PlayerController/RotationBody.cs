using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationBody : MonoBehaviour
{
    public Quaternion quaternion = new Quaternion(0, 0, 0, 0); // Текущее вращение
    public float qtrn; // Направление вращения


    void Update()
    {
        // Поворот персонажа
        if (qtrn != quaternion.y)
        {
            quaternion.y = Mathf.Lerp(quaternion.y, qtrn, Time.deltaTime * 5);
            if (Mathf.Abs(qtrn - quaternion.y) < 5)
                quaternion.y = qtrn;
            transform.rotation = Quaternion.Euler(quaternion.x, quaternion.y, quaternion.z);

        }
    }

    // Изменяет направление поворота
    public void RotatiBody(float eler)
    {
        qtrn = Mathf.Clamp(eler, 0, 180);
    }
}
