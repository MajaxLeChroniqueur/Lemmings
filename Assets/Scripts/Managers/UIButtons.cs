using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SA
{

    public class UIButtons : MonoBehaviour
    {
        public Ability ability;
        public Image buttonImg;
        public Text textUtilisation;
        public int nUtilisation;        
        private float timeScaled;
        public Transform parents;

        private void Update()
        {
            if(textUtilisation != null)
                textUtilisation.text = nUtilisation.ToString(""); 

            if(nUtilisation <= 0)
            {
                gameObject.SetActive(false);
            }            
        }

        public void Press()
        {
            Debug.Log("Test4");
            UIManager.singleton.PressAbilityButton(this);
        }

        public void FastForward()
        {
            timeScaled = GameManager.singleton.timeScaled;

            if (UnitManager.singleton.timeScale == UnitManager.singleton.originalTimeScale)
            {
                UnitManager.singleton.timeScale = timeScaled;
            }
            else
            {
                UnitManager.singleton.timeScale = UnitManager.singleton.originalTimeScale;
            }

            UIManager.singleton.PressAbilityButton(this);
        }

        public void Minus()
        {
            nUtilisation -= 1;
        }

        public void Explosion()
        {
            if (parents != null)
            {
                foreach (Transform lemmings in parents)
                {
                    if (lemmings.GetComponent<Unit>().isDead == false)
                    {
                        lemmings.GetComponent<Unit>().curAbility = Ability.explode;
                        lemmings.GetComponent<Unit>().anim.Play("dead");
                        lemmings.GetComponent<Unit>().isDead = true;

                    }
                    //lemmings.GetComponent<Unit>().Die();
                }
            }
        }
    }

}
