using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftHendController : MonoBehaviour
{
    [SerializeField] private Transform targetPosition; // В какой точке должна находится рука

    private Transform target; // Куда направленна рука
    


    void Update()
    {
        transform.position = targetPosition.position;
        transform.LookAt(target);
    }

    public void SetTarget(Transform target)
    {
        if (target != null)
        {
            this.target = target;
        }
    }
}
