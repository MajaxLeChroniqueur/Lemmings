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
        private float timeScaled;

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
    }

}
