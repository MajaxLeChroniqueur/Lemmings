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
        //public GameObject inGameCanvas;
        //public GameObject startCanvas;

        void Start()
        {
            Cursor.visible = false;
            //inGameCanvas.SetActive(true);
            //startCanvas.SetActive(false);
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.A))
            {
                UseKeyButton(Ability.stopper);
            }
            else if (Input.GetKeyDown(KeyCode.Z))
            {
                UseKeyButton(Ability.umbrella);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                UseKeyButton(Ability.dig_forward);
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                UseKeyButton(Ability.dig_down);
            }
            else if (Input.GetKeyDown(KeyCode.T))
            {
                UseKeyButton(Ability.builder);
            }
            else if (Input.GetKeyDown(KeyCode.Y))
            {
                UseKeyButton(Ability.archer);
            }
            else if (Input.GetKeyDown(KeyCode.U))
            {
                UseKeyButton(Ability.combattant);
            }
            else if (Input.GetKeyDown(KeyCode.I))
            {
                UseKeyButton(Ability.dig_diagonale);
            }
            else if (Input.GetKeyDown(KeyCode.O))
            {
                UseKeyButton(Ability.lighter);
            }
            else if (Input.GetKeyDown(KeyCode.P))
            {
                UseKeyButton(Ability.explode);
            }
        }

        public void Tick()
        {
            mouseTrans.transform.position = Input.mousePosition;

            if (overUnit)
            {
                mouse.sprite = box;
                if (Input.GetMouseButtonDown(0))
                {
                    //curButton.Minus();
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
            Debug.Log("Test");

            curButton = b;
            defColor = curButton.buttonImg.color;
            curButton.buttonImg.color = selectTint;
            if (curButton.ability != Ability.fast_forward)
            {
                Debug.Log("test2");
                targetAbility = curButton.ability;
            }
        }

        private void UseKeyButton(Ability a)
        {
            targetAbility = a;
        }

        public static UIManager singleton;
        void Awake()
        {
            singleton = this;
        }
    }
}
