using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundCam : MonoBehaviour
{
    public Transform target; // The target object to rotate around
    public float distance = 10.0f; // The distance from the target object
    public float height = 5.0f; // The height of the camera above the target object
    public float rotationSpeed = 1.0f; // The speed of rotation
    public LayerMask occlusionLayerMask; // The layer mask to use for occlusion detection

    private float currentRotationAngle = 0.0f; // The current rotation angle around the target object
    public float CamRot;

    void Update()
    {
        // Calculate the target position and rotation based on the current rotation angle
        Quaternion rotation = Quaternion.Euler(CamRot, currentRotationAngle, 0.0f);
        Vector3 position = rotation * new Vector3(0.0f, height, -distance) + target.position;

        // Check for occlusion between the camera and the target
        RaycastHit hitInfo;
        if (Physics.Linecast(target.position, position, out hitInfo, occlusionLayerMask))
        {
            // If there's an obstacle, move the camera closer to the target to avoid it
            float distanceToObstacle = Vector3.Distance(target.position, hitInfo.point);
            position = target.position + rotation * new Vector3(0.0f, height, -distanceToObstacle);
        }

        // Set the camera position and rotation to the calculated values
        transform.position = position;
        transform.rotation = rotation;

        // Update the current rotation angle
        currentRotationAngle += Time.deltaTime * rotationSpeed;
        if (currentRotationAngle > 360.0f)
        {
            currentRotationAngle -= 360.0f;
        }
    }



}
