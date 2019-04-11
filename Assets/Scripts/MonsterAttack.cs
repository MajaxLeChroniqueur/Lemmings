using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class MonsterAttack : MonoBehaviour {

        public Unit unit;
        public GameObject weapon;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Lemmings")
            {
                StartCoroutine(Attack());
            }
        }

        IEnumerator Attack()
        {
            unit.isAttacking = true;
            weapon.SetActive(true);
            yield return new WaitForSeconds(1f);
            weapon.SetActive(false);
            yield return new WaitForSeconds(1.5f);
            unit.isAttacking = false;
        }
    }
}
