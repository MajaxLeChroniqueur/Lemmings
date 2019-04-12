using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDetection : MonoBehaviour
{
    void OnTriggerEnter2D (Collider2D collider)
    {
        if(collider.gameObject.compareTag("Lemmings"))
        {
            collider.gameObject.setActive(false);
            LemmingsOutScript.scoreValue += 1;
        }
    }
}
