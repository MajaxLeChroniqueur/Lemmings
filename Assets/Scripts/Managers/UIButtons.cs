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

        public void Press()
        {
            UIManager.singleton.PressAbilityButton(this);
        }
    }
}
