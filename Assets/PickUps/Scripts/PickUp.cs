using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that provides collision detecting for pickups
/// </summary>

/*
    Class that provides collision detecting for pickups. Calls playerControl to increase speed by givin speed burst.

    @Author: Jussi Sepponen, Olli Korhonen
*/

[RequireComponent(typeof(Collider))]
public class PickUp : MonoBehaviour
{
    [Space(10)]
    public float rotationSpeed = 10.0f;

    [Space(6)]
    public float oscillationMax = 0.1f;
    public float oscillationSpeed = 1.0f;

    [Space(6)]
    public float currentOscillation = 0.0f;
    private float oscillationDirection = 1.0f;
    
    private Vector3 originalPosition;
    private Collider mainCollider;

    private Vector3 startingPosition;
    private Transform target;
    private float consumingLeft = 0.0f;
    private float consumeSpeed = 1.0f;

    private bool consuming = false;

    void Start()
    {
        this.originalPosition = transform.position;
        this.mainCollider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update ()
    {
        if (!this.consuming)
        {
            transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles + new Vector3(0, this.rotationSpeed * Time.deltaTime, 0));

            this.currentOscillation += oscillationDirection * oscillationSpeed * Time.deltaTime;
            this.oscillationDirection *= Mathf.Abs(currentOscillation) >= 1.0f ? -1 : 1; // Reverse oscillation if over limits
            this.currentOscillation = Mathf.Clamp(this.currentOscillation, -1.0f, 1.0f); // Clamp oscillation to -1.0f and 1.0f

            float value = Mathf.Pow(Mathf.Abs(this.currentOscillation), 1.0f / 1.2f) * Mathf.Sign(this.currentOscillation) * this.oscillationMax;

            transform.position = this.originalPosition + new Vector3(0, value, 0);
        }
        else if (this.consumingLeft > 0)
        {
            transform.localScale = new Vector3(this.consumingLeft, this.consumingLeft, this.consumingLeft);
            transform.position = Vector3.Lerp(target.position, startingPosition, this.consumingLeft);
            this.consumingLeft -= Time.deltaTime * consumeSpeed;
        }
        else
        {
            this.consumeSpeed = 0.0f;
            this.consuming = false;
            Destroy(this.gameObject);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("PickUp collided with player");
            Consumer consumer = other.gameObject.GetComponent<Consumer>();

            if (consumer == null)
            {
                Destroy(this.gameObject);
                return;
            }

            mainCollider.enabled = false;
            this.startingPosition = transform.position;
            this.target = consumer.getMouth();

            this.consumeSpeed = consumer.getConsumeSpeed();
            this.consumingLeft = 1.0f;
            this.consuming = true;
        }
    }
}


