using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explode : MonoBehaviour, IBehaviour
{
    public string explosionId;
    public int particleId;
    public float explosionRadius;

    private void OnMouseDown()
    {
        Debug.Log($"EXPLODE! explosionId: {explosionId} radius: {explosionRadius} particleId : {particleId}");
        Destroy(gameObject);
    }
}
