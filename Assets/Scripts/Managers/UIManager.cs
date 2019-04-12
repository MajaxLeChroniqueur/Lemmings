using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SA
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
        public GameObject inGameCanvas;
        public GameObject startCanvas;

        void Start()
        {
            Cursor.visible = false;
            inGameCanvas.SetActive(true);
            startCanvas.SetActive(false);
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
            if (curButton.ability != Ability.fast_forward)
            {
                targetAbility = curButton.ability;
            }

        }

        public static UIManager singleton;
        void Awake()
        {
            singleton = this;
        }
    }
}
