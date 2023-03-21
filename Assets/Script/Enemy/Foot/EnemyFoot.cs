using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFoot : MonoBehaviour
{
    [Header("Foot parametrs")]
    [SerializeField] [Range(0f, 20f)]
    private float distanse; // Дистанцию которую будет держать варг до игрока
    [SerializeField] [Range(0f, 10f)]
    private float speedMove; // Скорость перемещения врага
    [SerializeField] Transform startLeftRay; // Стартовая позиция левого луча
    [SerializeField] Transform startRightRay; // Стартовая позиция правого луча
    [SerializeField] float distanceRay; // Дистанция луча

    private Vector3 targetPosition; // Целевая позиция
    private Vector3 vectorMove; // Направление для перемещения
    private Vector3 curentPosition; // Текущая позиция
    public Vector3 toPositionRay; // Позиция до которой пускается луч

    private Transform playerTransform;
    private Transform curentTransform;
    private RaycastHit raycastHit;
    private Ray ray;


    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        targetPosition = playerTransform.position;
        curentTransform = transform.parent;
        curentPosition = curentTransform.position;
    }


    void FixedUpdate()
    {
        vectorMove = curentTransform.position - playerTransform.position;

        // Проверка на возможность передвижения
        if (vectorMove.z > 0)
        {
            toPositionRay.x = startLeftRay.position.x;
            toPositionRay.y = startLeftRay.position.y;
            toPositionRay.z = startLeftRay.position.x - distanceRay;
            ray = new Ray(startLeftRay.position, toPositionRay);
            if (Physics.Raycast(ray, out raycastHit, distanceRay))
            {
                return;
            }
            
        }
        else
        {
            toPositionRay.x = startRightRay.position.x;
            toPositionRay.y = startRightRay.position.y;
            toPositionRay.z = startRightRay.position.x + distanceRay;
            ray = new Ray(startRightRay.position, toPositionRay);
            if (Physics.Raycast(ray, out raycastHit, distanceRay))
            {
                return;
            }
        }


        // Движение по горизонтали
        if (Mathf.Abs(vectorMove.z) > distanse)
        {
            curentPosition.z = vectorMove.z;
            curentPosition.y = curentTransform.position.y;

            curentTransform.Translate(curentPosition.normalized * -(speedMove / 100));
        }
    }

    public void OnDestroyFoot()
    {
        Destroy(gameObject);
    }
}
