using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class UnitManager : MonoBehaviour
    {
        public float maxUnits = 10;
        public float timeScale = 1;
        float delta;
        public float interval = 1;
        float timer;
        public GameObject unitPrefab;
        GameObject unitsParent;
        List<Unit> all_units = new List<Unit>();
        GameManager gameManager;

        public bool changeSpeed;

        public static UnitManager singleton;
        void Awake()
        {
            singleton = this;
        }

        void Start()
        {
            unitsParent = new GameObject();
            unitsParent.name = "Units Parents";
            gameManager = GameManager.singleton;
        }

        void Update()
        {
            delta = Time.deltaTime;
            delta *= timeScale;

            if (changeSpeed)
            {
                changeSpeed = false;
                ChangeSpeedForAllUnits(timeScale);
            }

            if(all_units.Count < maxUnits)
            {
                timer -= delta;
                if (timer < 0)
                {
                    timer = interval;
                    SpawnLennings();
                }
            }            

            for (int i = 0; i < all_units.Count; i++)
            {
                all_units[i].Tick(delta);
            }
        }

        void SpawnLennings()
        {
            GameObject g = Instantiate(unitPrefab);
            g.transform.parent = unitsParent.transform;
            Unit u = g.GetComponent<Unit>();
            u.Init(gameManager);
            all_units.Add(u);
            u.move = true;
        }

        void ChangeSpeedForAllUnits(float t)
        {
            for (int i = 0; i < all_units.Count; i++)
            {
                all_units[i].anim.speed = t;
            }
        }

        public Unit GetClosest(Vector3 o)
        {
            Unit r = null;
            float minDist = 5f * 0.01f;

            for (int i = 0; i < all_units.Count; i++)
            {
                float tempDist = Vector3.Distance(o, all_units[i].transform.position);
                if (tempDist < minDist)
                {
                    minDist = tempDist;
                    r = all_units[i];
                }
            }

            return r;
        }
    }

    public enum Ability
    {
        walker, stopper, umbrella, dig_forward, dig_down, explode, dead, filler, builder, dig_diagonale, archer
    }
}
