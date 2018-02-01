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

    void Start()
    {
        this.originalPosition = transform.position;
    }

    // Update is called once per frame
    void Update ()
    {
        transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles + new Vector3(0, this.rotationSpeed * Time.deltaTime, 0));

        this.currentOscillation += oscillationDirection * oscillationSpeed * Time.deltaTime;
        this.oscillationDirection *= Mathf.Abs(currentOscillation) >= 1.0f ? -1 : 1; // Reverse oscillation if over limits
        this.currentOscillation = Mathf.Clamp(this.currentOscillation, -1.0f, 1.0f); // Clamp oscillation to -1.0f and 1.0f

        float value = Mathf.Pow(Mathf.Abs(this.currentOscillation), 1.0f / 1.2f) * Mathf.Sign(this.currentOscillation) * this.oscillationMax;

        transform.position = this.originalPosition + new Vector3(0, value, 0);
    }

    float CubeRoot(float d)
    {
        return Mathf.Pow(Mathf.Abs(d), 1f / 3f) * Mathf.Sign(d);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //TODO something
            Destroy(this.gameObject);
        }
    }
}


