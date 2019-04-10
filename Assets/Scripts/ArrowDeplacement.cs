using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowDeplacement : MonoBehaviour {

    private Vector2 direction;
    [SerializeField] private float speed;
    public GameObject objetVisee;
    private float delta;

    private void Start()
    {
        direction = (new Vector2(objetVisee.transform.position.x, objetVisee.transform.position.y) - new Vector2 (transform.position.x,transform.position.y)).normalized;
        Destroy(gameObject, 10f);
    }

    private void FixedUpdate()
    {
        transform.position = transform.position + (new Vector3(direction.x,direction.y,0) * speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Enemy")
        {
            Destroy(gameObject, 0.05f);
        }
    }
}
