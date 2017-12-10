using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public Text countDownText;
    public Countdown timeoutCountdown; //Countdown script that displays a decreasing counter that calls a predefined function of the gamemanager after it finishes
    public Timer timeOutput; //Timer script that displays time with input of seconds passed
    public ResultPanel resultTimePanel; //UI text after final, shows result time

    private void Awake()
    {
        if (FindObjectsOfType<UIManager>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        resultTimePanel.gameObject.SetActive(false);
        timeOutput.gameObject.SetActive(false);
    }
}
