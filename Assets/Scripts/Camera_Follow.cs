using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform cameraTransform; // Camera transform
    public float defaultDistanceFromCamera = 2.0f; // Distance from camera
    public Vector3 panelOffset = new Vector3(0, 0, 0); // Canvas AND Camera offset
    public LayerMask obstacleLayers; // Layer mask to define which objects can block the canvas
    public float minimumDistanceFromCamera = 0.5f; // Minimum distance to avoid clipping with the camera

    void Update()
    {
        // Check if the camera transform is assigned
        Vector3 targetPosition = cameraTransform.position + cameraTransform.forward * defaultDistanceFromCamera;
        transform.position = targetPosition + panelOffset;

        // Perform a raycast to check for obstacles between the camera and the canvas
        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, defaultDistanceFromCamera, obstacleLayers))
        {
            // If an obstacle is detected, adjust the canvas position to be just in front of the obstacle
            float distanceToObstacle = hit.distance;
            // Ensure the canvas doesn't get too close to the camera (avoid going below minimum distance)
            targetPosition = cameraTransform.position + cameraTransform.forward * Mathf.Max(minimumDistanceFromCamera, distanceToObstacle - 0.1f);
        }
        
        // Update the canvas position
        transform.position = targetPosition;
        
        // Rotate the canvas to face the camera
        transform.rotation = Quaternion.LookRotation(transform.position - cameraTransform.position);
    }
}