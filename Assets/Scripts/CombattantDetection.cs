using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SA
{
    public class CombattantDetection : MonoBehaviour
    {
        public GameObject weapon;
        private float cooldown;
        public Unit unit;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(!unit.isAttacking)
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
            yield return new WaitForSeconds(0.5f);
            unit.isAttacking = false;
        }
    }
}
