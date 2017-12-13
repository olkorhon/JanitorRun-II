using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Animator))]
public class Steering : MonoBehaviour
{
    public float verticalDeadzone = 0.1f;
    public float speed = 0.0f;

    public float horizontalDeadzone = 0.1f;
    public float steer = 0.0f;
    Animator animator;

    public Animator modelAnimator;

	// Use this for initialization
	void Start () {
        this.animator = GetComponent<Animator>();

        if (modelAnimator == null)
        {
            Debug.LogWarning("No model animator linked to vehicle steering script");
        }
	}
	
	// Update is called once per frame
	void Update () {
        // Fetch input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        this.steer = horizontal;
        // Propagate horizontal input to current control
        //if (horizontal > verticalDeadzone) this.steer += horizontal * Time.deltaTime;
        //else if (horizontal < -verticalDeadzone) this.steer += horizontal * Time.deltaTime;
        //else this.steer *= (1.0f - 0.50f);
        //this.steer = Mathf.Clamp(this.steer, -1, 1);
        
        // Propagate vertical input to current control
        if      (vertical >  verticalDeadzone) this.speed += vertical * Time.deltaTime * 0.5f;
        else if (vertical < -verticalDeadzone) this.speed += vertical * Time.deltaTime * 0.5f;
        else                                   this.speed *= (1.0f - (0.1f * Time.deltaTime));
        this.speed = Mathf.Clamp(this.speed, -0.5f, 1.0f);

        // Update animator
        this.animator.SetFloat("steer", this.steer);
        this.animator.SetFloat("speed", this.speed);

        if (modelAnimator)
        {
            modelAnimator.SetFloat("forward", vertical);
            modelAnimator.SetFloat("steer", horizontal);
        }
	}
}
