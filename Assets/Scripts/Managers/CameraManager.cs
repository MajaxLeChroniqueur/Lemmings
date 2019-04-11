using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lemmings
{
    public class CameraManager : MonoBehaviour
    {
        public float moveSpeed = 0.01f;
        public Transform camTrans;

        public float minY;
        public float minX;
        public float maxX;

        void Update()
        {
            float h = Input.GetAxis("Horizontal");
            Vector3 mp = Vector3.zero;
            mp.x = h * moveSpeed;

            Vector3 tp = camTrans.position;
            tp += mp;
            tp.x = Mathf.Clamp(tp.x, minX, maxX + 0.01f);
            camTrans.position = tp;
        }
    }
}
