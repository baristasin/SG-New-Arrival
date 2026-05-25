using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform mainCameraTransform;

    void Start()
    {
        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }
    }

    void LateUpdate()
    {
        if (mainCameraTransform != null)
        {
            //transform.rotation = mainCameraTransform.rotation;
            transform.LookAt(transform.position + mainCameraTransform.forward);
        }
    }
}