using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class LanternScript : MonoBehaviour {

        public GameObject light;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.GetComponent<Unit>() != null && collision.gameObject.GetComponent<Unit>().doesMakeLight && !light.activeSelf)
            {
                light.SetActive(true);
            }
        }
    }
}
