using UnityEngine;

public class LockRotation : MonoBehaviour
{
    private float lockedZRotation;

    void Start()
    {
        // Store the initial Z rotation
        lockedZRotation = 0;
    }

    void LateUpdate()
    {
        // Keep the Z rotation locked while allowing X and Y to change
        Vector3 currentRotation = transform.eulerAngles;
        currentRotation.z = lockedZRotation;
        transform.eulerAngles = currentRotation;
    }
}
