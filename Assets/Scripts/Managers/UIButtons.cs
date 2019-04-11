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

        private void Update()
        {
            textUtilisation.text = nUtilisation.ToString(""); 

            if(nUtilisation <= 0)
            {
                gameObject.SetActive(false);
            }            
        }

        public void Press()
        {
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
    }

}
