using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class EnemiVolant : MonoBehaviour {

        public List<Transform> tourDeGarde;
        public bool hasSpotedSomething = false;
        private bool isAttacking = false;
        private bool isCommingBack = false;
        private int currentPoint;

        public Transform target;

        public float speed = 0.5f;

        private Vector3 direction;

        public float timer = 1f;

        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }

            if (isAttacking)
            {
                if (Vector3.Distance(target.position, transform.position) <= 0.05f)
                {
                    isAttacking = false;
                    isCommingBack = true;
                    target.transform.gameObject.GetComponent<Unit>().Die();
                }
                direction = (target.position - transform.position).normalized;
            }
            else if (isCommingBack)
            {
                direction = (tourDeGarde[currentPoint].position - transform.position).normalized;
                if (Vector3.Distance(tourDeGarde[currentPoint].position, transform.position) <= 0.05f)
                {
                    isCommingBack = false;
                }
            }
            else if (hasSpotedSomething)
            {
                direction = Vector3.zero;
                if (timer <= 0)
                {
                    hasSpotedSomething = false;
                    isAttacking = true;
                }
            }
            else
            {
                direction = (tourDeGarde[currentPoint].position - transform.position).normalized;
                if (Vector3.Distance(tourDeGarde[currentPoint].position, transform.position) <= 0.05f)
                {
                    currentPoint++;
                    if (currentPoint >= tourDeGarde.Count)
                    {
                        currentPoint = 0;
                    }
                }

            }

            transform.position = transform.position + direction * speed * Time.deltaTime;
        }
    }
}
