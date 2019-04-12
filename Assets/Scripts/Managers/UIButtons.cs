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

        public Color color = Color.gray;

        Button btn;

        bool canUse = true;

        private void Start()
        {
            btn = gameObject.GetComponent<Button>();
        }

        private void Update()
        {
            if (canUse == true)
            {
                textUtilisation.text = nUtilisation.ToString("");
            }

            if (nUtilisation <= 0)
            {
                btn.interactable = false;
                buttonImg.color = color;

                canUse = false;
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

        public void ExplodeAll()
        {
            UnitManager unitManager = UnitManager.singleton;

            foreach (Unit unit in unitManager.all_units)
            {
                unit.curAbility = Ability.explode;
            }
        }
    }
}
