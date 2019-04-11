using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class DetectionEnnemiVolant : MonoBehaviour
    {
        public EnemiVolant comportement;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "Lemmings" && !comportement.hasSpotedSomething)
            {
                comportement.target = collision.transform;
                comportement.timer = 2;
                comportement.hasSpotedSomething = true;
            }
        }
    }
}
