using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultsUpdater : MonoBehaviour
{
    public Sprite starEmpty;
    public Sprite starFilled;

    public Image[] starImages = new Image[5];

    public float[] times = new float[] { 80f, 69f, 66f, 64f, 61f };

    private void OnEnable()
    {
        if (GameManagerScript.Instance == null)
            return;

        double resultTime = GameManagerScript.Instance.resultTime;

        for (int i = 0; i < times.Length; i++)
        {
            if (times[i] > resultTime)
            {
                starImages[i].sprite = starFilled;
            }
            else
            {
                starImages[i].sprite = starEmpty;
            }
        }
    }
}
