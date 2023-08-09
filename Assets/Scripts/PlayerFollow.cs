using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;

    [SerializeField] private float smoothSpeed;


    private void LateUpdate()
    {
        if (target == null)
            return;


        Vector3 cameraPosition = target.position + offset;

        Vector3 smoothPosition = Vector3.Lerp(transform.position, cameraPosition, smoothSpeed);

        transform.position = smoothPosition;
        transform.LookAt(target);
    }
}
