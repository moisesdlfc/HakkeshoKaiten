using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{

    // Singleton
    public static CameraFollower sharedInstance;

    // Objective to follow
    public GameObject followTarget;

    // Smoothness lerp
    public float movementSmoothness = 1.0f;
    public float rotationSmoothness = 1.0f;

    // Enable/disable tracing
    public bool canFollow = true;

    private void Awake()
    {
        // Reference sharedInstance
        sharedInstance = this;
    }

    private void LateUpdate()
    {
        if (followTarget == null || canFollow == false)
        {
            return;
        }

        // Transform camera position and rotation to follow target
        this.transform.position = Vector3.Lerp(this.transform.position, followTarget.transform.position, (Time.deltaTime * movementSmoothness));
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, followTarget.transform.rotation, (Time.deltaTime * rotationSmoothness));
    }
}
