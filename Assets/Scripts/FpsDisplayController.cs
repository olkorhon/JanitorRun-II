using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class FpsDisplayController : MonoBehaviour
{
    private Text text;
    public float cur_delay;

	// Use this for initialization
	void Start ()
    {
        text = GetComponent<Text>();
        cur_delay = 0.0f;
	}
	
	// Update is called once per frame
	void Update ()
    {
        cur_delay = cur_delay * 0.9f + Time.deltaTime * 0.1f;
	}

    void FixedUpdate()
    {
        text.text = (1.0f / cur_delay).ToString();
    }
}
