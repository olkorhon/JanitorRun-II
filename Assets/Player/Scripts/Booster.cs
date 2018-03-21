using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Booster : MonoBehaviour
{
    [Space(10)]
    private Animator animator;
	public Animator janitorAnimator;

    public float boostMultiplier = 2.0f;
    public float maxBoostDuration = 5.0f;
    public float windupDuration = 1.0f;

    [Space(6)]
    bool isBoosting = false;
    public float boostLeft = 0.0f;

    // Use this for initialization
    void Start ()
    {
        this.animator = GetComponent<Animator>();
	}

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pick Up"))
        {
            activateBoost();
        }
    }

    // Update is called once per frame
    void Update ()
    {
        if (!this.isBoosting)
            return;

        if (boostLeft > 0.0f)
        {
            if (boostLeft < windupDuration)
            {
                float normalisedValue = this.boostLeft / this.windupDuration;
                this.animator.speed = 1.0f + normalisedValue * (this.boostMultiplier - 1.0f);
				this.janitorAnimator.speed = 1.0f + normalisedValue * (this.boostMultiplier - 1.0f);
            }
            else if (boostLeft > maxBoostDuration - windupDuration)
            {
                float normalisedValue = 1.0f - (this.boostLeft - this.maxBoostDuration + this.windupDuration) / this.windupDuration;
                this.animator.speed = 1.0f + normalisedValue * (this.boostMultiplier - 1.0f);
				this.janitorAnimator.speed = 1.0f + normalisedValue * (this.boostMultiplier - 1.0f);
            }
            else
            {
                this.animator.speed = 1.0f + (this.boostMultiplier - 1.0f);
				this.janitorAnimator.speed = 1.0f + (this.boostMultiplier - 1.0f);
            }

            boostLeft -= Time.deltaTime;
        }
        else
        {
            this.animator.speed = 1.0f;
			this.janitorAnimator.speed = 1.0f;
            this.boostLeft = 0;

            this.isBoosting = false;
        }
	}

    public void activateBoost()
    {
        if (this.isBoosting)
        {
            if (boostLeft < this.maxBoostDuration - this.windupDuration)
            {
                this.boostLeft = this.maxBoostDuration;
                this.boostLeft -= boostLeft < this.windupDuration ? this.boostLeft : this.windupDuration;
            }
        }
        else
        {
            this.boostLeft = this.maxBoostDuration;
        }

        this.isBoosting = true;
    }
}
