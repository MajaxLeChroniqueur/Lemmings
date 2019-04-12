using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingCameraScript : MonoBehaviour
{
    public float limiteGauche;
    public float limiteDroite;

    public float speed;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.RightArrow) && transform.position.x < limiteDroite)
        {
            transform.position += Vector3.right * Time.deltaTime * speed;
        }
        else if(Input.GetKey(KeyCode.LeftArrow) && transform.position.x > limiteGauche)
        {
            transform.position -= Vector3.right * Time.deltaTime * speed;
        }
    }
}
