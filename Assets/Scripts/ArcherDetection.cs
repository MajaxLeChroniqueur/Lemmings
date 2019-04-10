using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherDetection : MonoBehaviour {

    public GameObject arrow;
    private float tps = 0f;

    private void Update()
    {
        if(tps>0)
        {
            tps -= Time.deltaTime;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.gameObject.tag == "Enemy" && tps <= 0)
        {
            ShootArrow(other.gameObject.transform.position, other.gameObject);
            tps = 2f;
        }
    }

    private void ShootArrow(Vector2 position, GameObject other)
    {
        GameObject arrowScript = Instantiate(arrow, transform.position, Quaternion.identity);
        arrowScript.transform.up = position;
        if(arrow.GetComponent<ArrowDeplacement>() != null)
        {
            arrow.GetComponent<ArrowDeplacement>().objetVisee = other;
        }
    }
}
