using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lemmings
{
    public class UIButtons : MonoBehaviour
    {
        public Ability ability;
        public Image buttonImg;
        public Text textUtilisation;
        public int nUtilisation;        

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

        public void Minus()
        {
            nUtilisation -= 1;
        }
    }
}
