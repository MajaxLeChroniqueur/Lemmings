using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lemmings
{
    public class ExitPoint : MonoBehaviour
    {
        public UIManager uIManager;

        private void Start()
        {
            uIManager = UIManager.singleton;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Units")
            {
                collision.gameObject.SetActive(false);
                uIManager.lemmingsOut++;
            }
        }

    }
}
