using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LemmingsOutScript : MonoBehaviour
{
    public static int scoreValue = 0;
    public int scoreToWin = 10;
    Text score;

    // Start is called before the first frame update
    void Start()
    {
        score = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        score.text = "LEMMINGS OUT: " + scoreValue;


        if (scoreValue >= scoreToWin)
        {
            MenuPanels.EnoughLemmingsHavePassed = true;
        }

    }
}
