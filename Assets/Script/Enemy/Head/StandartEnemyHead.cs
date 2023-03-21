using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandartEnemyHead : MonoBehaviour
{
    public void OnDestroyHead()
    {
        Destroy(gameObject);
    }
}
