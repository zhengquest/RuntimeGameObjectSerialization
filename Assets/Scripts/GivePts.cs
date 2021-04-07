using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GivePts : MonoBehaviour, IBehaviour 
{
    public int points;
    
    private void OnMouseDown()
    {
        Debug.Log($"GET POINTS {points}");
        Destroy(gameObject);
    }
}
