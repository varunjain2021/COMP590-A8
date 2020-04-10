using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class redirection : MonoBehaviour
{
    public GameObject Camera; 
    public GameObject VRTrackingOrigin;
    public Vector3 prevForwardVector;
    public Vector3 prevLocation;
    public GameObject pivot;
    public float prevYawRelativeToCenter;
    public float decelerateThreshold;
    public float accelerateThreshold;
    public float translationalGainThreshold;
    public TextMesh data;

    public float d(Vector3 A, Vector3 B, Vector3 C) {
        float side = ((A.x - B.x) * (C.z - B.z)) - ((A.z - B.z) * (C.x - B.x));
        return side;     
    }
    public float angleBetweenVectors(Vector3 A, Vector3 B) {

        A.y = 0f;
        B.y = 0f;

        float angle = (Mathf.Acos(Vector3.Dot(Vector3.Normalize(A), Vector3.Normalize(B)))) * Mathf.Rad2Deg;
        return angle;

    }
    
    // Start is called before the first frame update
    void Start()
    {
        prevForwardVector = Camera.transform.forward;
        prevLocation = Camera.transform.position;
        prevYawRelativeToCenter = angleBetweenVectors(Camera.transform.forward, VRTrackingOrigin.transform.position-Camera.transform.position);
        //Application.targetFrameRate = 30;
    }
    // Update is called once per frame
    void Update()

    {

        float howMuchUserRotated = angleBetweenVectors(prevForwardVector, Camera.transform.forward);
        float directionUserRotated = ((d((Camera.transform.position + prevForwardVector), Camera.transform.position, Camera.transform.position + Camera.transform.forward) < 0) ? 1 : -1) * -1;
        float deltaYawRelativeToCenter=prevYawRelativeToCenter-angleBetweenVectors(Camera.transform.forward, VRTrackingOrigin.transform.position- Camera.transform.position);
        //>0 means that user is rotating towards PE center (so we should accelerate)
        float distanceFromCenter=(Camera.transform.position-VRTrackingOrigin.transform.position).magnitude; //[or (Camera.transform.position-VRTrackingOrigin.transform.position).magnitude]
        float longestDimensionOfPE = 0.5f; 

        float howMuchToAccelerate=((deltaYawRelativeToCenter<0)? -decelerateThreshold: accelerateThreshold) * howMuchUserRotated * directionUserRotated * Mathf.Clamp(distanceFromCenter/longestDimensionOfPE/2,0,1);

        if (!float.IsNaN(howMuchToAccelerate))
        {
            VRTrackingOrigin.transform.RotateAround(Camera.transform.position, new Vector3(0f, 1f, 0f), howMuchToAccelerate);
        }

        prevForwardVector=Camera.transform.forward;
        prevYawRelativeToCenter= angleBetweenVectors(Camera.transform.forward,(VRTrackingOrigin.transform.position- Camera.transform.position));


        // EXTRA CREDIT:
        Vector3 trajectoryVector = Camera.transform.position - prevLocation;
        Vector3 howMuchToTranslate = Vector3.Normalize(trajectoryVector) * translationalGainThreshold;
        VRTrackingOrigin.transform.position += howMuchToTranslate;
        prevLocation = Camera.transform.position;

        data.text = "Varun Jain";
    }
}
