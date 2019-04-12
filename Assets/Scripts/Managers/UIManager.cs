using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Lemmings
{
    public class UIManager : MonoBehaviour
    {
        public Transform mouseTrans;
        public Image mouse;
        public Sprite cross1;
        public Sprite cross2;
        public Sprite box;
        public bool overUnit;
        public bool switchToAbility;
        public Ability targetAbility;
        public UIButtons curButton;
        public Color selectTint;
        Color defColor;
        public GameObject canvas;

        public Text lemmingsOutText;
        [HideInInspector] public int lemmingsOut;

        UnitManager unitManager;
        public Text lemmingLeftText;

        void Start()
        {
            unitManager = UnitManager.singleton;

            Cursor.visible = false;
            canvas.SetActive(true);

            lemmingsOut = 0;            
        }

        private void Update()
        {
            LemmingsOut();
            LemmingsLeft();
            
            if (curButton.nUtilisation < 0)
            {
                targetAbility = Ability.walker;
            }
        }

        void LemmingsOut()
        {
            lemmingsOutText.text = lemmingsOut.ToString("00");
        }

        void LemmingsLeft()
        {
            lemmingLeftText.text = unitManager.lemmingsLeft.ToString("00");
        }

        public void Tick()
        {
            mouseTrans.transform.position = Input.mousePosition;

            if (overUnit)
            {
                mouse.sprite = box;
                if (Input.GetMouseButtonDown(0))
                {
                    curButton.Minus();
                }
            }
            else
            {
                mouse.sprite = cross1;
            }
        }

        public void PressAbilityButton(UIButtons b)
        {
            if (curButton)
            {
                curButton.buttonImg.color = defColor;
            }

            curButton = b;
            defColor = curButton.buttonImg.color;
            curButton.buttonImg.color = selectTint;
            targetAbility = curButton.ability;
        }        

        public static UIManager singleton;
        void Awake()
        {
            singleton = this;
        }
    }
}
