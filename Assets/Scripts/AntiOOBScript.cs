using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class AntiOOBScript : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "Lemmings")
            {
                collision.GetComponent<Unit>().Die();
            }
        }
    }
}
