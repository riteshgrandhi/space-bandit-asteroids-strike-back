using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    public float time = 1.5f;
    void Awake()
    {
        Destroy(gameObject, time);
    }
}
