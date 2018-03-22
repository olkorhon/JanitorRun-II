using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextTinter : MonoBehaviour
{
    Text text;

	// Use this for initialization
	void Start ()
    {
        text = GetComponent<Text>();
	}

    public void makeGray()
    {
		text.color = new Color(0.75f, 0.75f, 0.75f);
    }
    public void makeBlack()
    {
        text.color = Color.black;
    }
}
