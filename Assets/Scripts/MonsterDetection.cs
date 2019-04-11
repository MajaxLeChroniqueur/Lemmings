using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDetection : MonoBehaviour {

    public GameObject weapon;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Lemmings" && !weapon.activeSelf)
        {
            StartCoroutine(Attack(2f));
        }
    }

    IEnumerator Attack(float seconds)
    {
        weapon.SetActive(true);
        yield return new WaitForSeconds(seconds);
        weapon.SetActive(false);
    }
}
