using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour {

    [SerializeField] private int maxHealth;
    private int health;

	// Use this for initialization
	void Start () {
        health = maxHealth;
	}

    private void Update()
    {
        if(health<=0)
        {
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "PlayerAttack")
        {
            health--;
        }
    }

    void Die()
    {
        //Animation de mort;
        Destroy(gameObject);
    }
}
