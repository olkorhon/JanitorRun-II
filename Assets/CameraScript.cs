
using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour
{
    [SerializeField]
    public Transform target = null;
    [SerializeField]
    private float distance = 3.0f;
    [SerializeField]
    private float height = 1.0f;

    [SerializeField]
    private float bumperDistanceCheck = 2.25f; // length of bumper ray
    [SerializeField]
    private float bumperCameraHeight = 1.8f; // adjust camera height while bumping
    [SerializeField]
    private Vector3 bumperRayOffset; // allows offset of the bumper ray from target origin

    [Tooltip("If character speed is below this value, then the camera will default to looking forwards.")]
    public float rotationThreshold = 1f;

    [Tooltip("How closely the camera matches the character's velocity vector. The lower the value, the smoother the camera rotations, but too much results in not being able to see where you're going.")]
    public float cameraRotationSpeed = 5.0f;

    private Rigidbody janitorPhysics;
    private Transform janitor;

    /// <Summary>
    /// If the target moves, the camera should child the target to allow for smoother movement. DR
    /// </Summary>
    private void Start()
    {
        //executeDelayed(2);
        //Camera.main.transform.parent = target;
        janitor = GameObject.FindGameObjectWithTag("Player").transform;
        janitorPhysics = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
    }
    IEnumerator executeDelayed(float time)
    {
        yield return new WaitForSeconds(time);

        PlayerObject[] players = FindObjectsOfType<PlayerObject>();
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].isLocalPlayer)
            {
                Camera.main.transform.parent = players[i].transform;
                janitor = players[i].transform;
                janitorPhysics = players[i].GetComponent<Rigidbody>();
            }

        }

    }

    private void LateUpdate()
    {
        //Change the forward vector to x-axis, should be fixed by rotating model in blender
        Vector3 new_forward =  Quaternion.Euler(2, 90, 0) * janitor.forward;

        Vector3 wantedPosition = target.TransformPoint(-distance, height, 0);
        Quaternion look;

        // check to see if there is anything behind the target
        RaycastHit hit;
        Vector3 back = target.transform.TransformDirection(-1 * Vector3.forward);

        // cast the bumper ray out from rear and check to see if there is anything behind
        if (Physics.Raycast(target.TransformPoint(bumperRayOffset), back, out hit, bumperDistanceCheck)
            && hit.transform != target) // ignore ray-casts that hit the user. DR
        {
            // clamp wanted position to hit position
            wantedPosition.x = hit.point.x;
            wantedPosition.z = hit.point.z;
            wantedPosition.y = Mathf.Lerp(hit.point.y + bumperCameraHeight, wantedPosition.y, Time.deltaTime * cameraRotationSpeed);
        }

        //Moves the camera to match the Janitor's position
        transform.position = Vector3.Lerp(transform.position, wantedPosition, Time.deltaTime * cameraRotationSpeed);

        // If the car isn't moving, default to looking forwards. Prevents camera from freaking out with a zero velocity getting put into a Quaternion.LookRotation
        if (janitorPhysics.velocity.magnitude < rotationThreshold)
        {
            look = Quaternion.LookRotation(new_forward);
        }
        else
        {
            look = Quaternion.LookRotation(janitorPhysics.velocity.normalized, target.up);
        }
        

        // Rotate the camera towards the velocity vector.
        look = Quaternion.Slerp(transform.rotation, look, cameraRotationSpeed * Time.deltaTime);
        transform.rotation = look;

    }
}

