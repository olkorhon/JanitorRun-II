using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LikertKnob : MonoBehaviour
{
    public int order;
    LikertController controller;

	// Use this for initialization
	void Start ()
    {
        controller = GetComponentInParent<LikertController>();		
	}

    // Tell controller to select this component
    public void selectMe()
    {
        this.controller.changeSelection(order);
    }
}
