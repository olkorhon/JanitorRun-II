using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Animator))]
public class Steering : MonoBehaviour
{
    public float speed = 0.0f;
    public float steer = 0.0f;

    public float verticalDeadzone = 0.1f;
    public float speedDampening = 1.0f;
    public float speedDifferenceThreshold = 1.0f;

    // Optional trackers
    public DiffTracker diffTracker;

    // Animators for board and player
    private Animator scooterAnimator;
    public Animator janitorAnimator;

    // Variables used to keep track of actual speed and rootmotion speed
    private Vector3 lastPosition;
    private Rigidbody rigidBody;

    public int diffBufferSize = 5;
    private float[] diffBuffer;
    private int diffBufferPos = 0;

    // Use this for initialization
    void Start ()
    {
        this.scooterAnimator = GetComponent<Animator>();
        this.rigidBody = GetComponent<Rigidbody>();

        if (janitorAnimator == null)
        {
            Debug.LogWarning("No model animator linked to vehicle steering script");
        }

        this.diffBuffer = new float[diffBufferSize];
	}
	
	// Update is called once per frame
	void Update ()
    {
        // Fetch input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Propagate vertical input to current control
        if (Mathf.Abs(verticalInput) > verticalDeadzone)
            this.speed += verticalInput * Time.deltaTime;
        else
            dampenSpeed();

        // Update animation controlling variables
        this.speed = Mathf.Clamp(this.speed, -0.5f, 1.0f);
        this.steer = horizontalInput;

        // Update scooter animator
        this.scooterAnimator.SetFloat("steer", this.steer);
        this.scooterAnimator.SetFloat("speed", this.speed);

        // Update janitor animator
        if (janitorAnimator)
        {
            janitorAnimator.SetFloat("forward", verticalInput);
            janitorAnimator.SetFloat("steer", horizontalInput);
        }
	}

    private void dampenSpeed()
    {
        if (this.speed > 0)
        {
            if (this.speed > this.speedDampening * Time.deltaTime)
                this.speed -= this.speedDampening * Time.deltaTime;
            else
                this.speed = 0;
        }
        else
        {
            if (this.speed < -this.speedDampening * Time.deltaTime)
                this.speed += this.speedDampening * Time.deltaTime;
            else
                this.speed = 0;
        }
    }

    private void LateUpdate()
    {
        float actualDeltaSpeed = (this.transform.position - this.lastPosition).magnitude * (1.0f / Time.deltaTime);
        float difference = actualDeltaSpeed - rigidBody.velocity.magnitude;

        diffBuffer[diffBufferPos] = difference;
        diffBufferPos = (diffBufferPos + 1) % diffBufferSize;

        // Only correct speed when the difference is too big to ignore
        bool exceptionFound = false;
        for (int i = 0; i < diffBufferSize; i++)
        {
            if (Mathf.Abs(diffBuffer[i]) < this.speedDifferenceThreshold)
            {
                exceptionFound = true;
                break;
            }
        }

        if (!exceptionFound)
            this.speed *= actualDeltaSpeed / rigidBody.velocity.magnitude;

        this.lastPosition = this.transform.position;

        if (diffTracker != null)
        {
            diffTracker.advance(actualDeltaSpeed, rigidBody.velocity.magnitude, difference);
        }
    }
}
