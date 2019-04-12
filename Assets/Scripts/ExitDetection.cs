using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDetection : MonoBehaviour
{
    void OnTriggerEnter2D (Collider2D collider)
    {
        if(collider.gameObject.CompareTag("Lemmings"))
        {
            collider.gameObject.SetActive(false);
            LemmingsOutScript.scoreValue += 1;
        }
    }
}
